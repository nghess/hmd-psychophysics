Shader "Unlit/DriftingSineWaveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Frequency ("Frequency", Float) = 10.0
        _Contrast ("Contrast", Float) = 1.0
        _Speed ("Speed", Float) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // Make the shader work with 2D textures
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Frequency;
            float _Contrast;
            float _Speed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Time-dependent phase shift for drifting
                float phase = _Time.y * _Speed;
                
                // Sine wave calculation for grating
                float sineValue = sin((i.uv.x + phase) * _Frequency * 2 * 3.14159265);
                
                // Adjusting Contrast (contrast)
                sineValue = sineValue * _Contrast * 0.5 + 0.5;
                
                // Gaussian mask
                // Calculate the distance from the center (assuming uv coordinates are [0, 1], center is at [0.5, 0.5])
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                
                // Parameters for the Gaussian function
                float sigma = 0.1; // Controls the spread of the Gaussian
                float gaussianValue = exp(-((dist * dist) / (2.0 * sigma * sigma)));

                // Apply the Gaussian mask by blending towards 50% gray based on the Gaussian value
                // Define 50% gray
                float4 gray = float4(0.5, 0.5, 0.5, 1.0);
                
                // Calculate the sine wave color
                float4 sineColor = tex2D(_MainTex, i.uv) * sineValue;
                
                // Interpolate between the sine wave color and 50% gray based on the gaussianValue
                float4 finalColor = lerp(gray, sineColor, gaussianValue);
                
                return finalColor;
            }
            ENDCG
        }
    }
}
