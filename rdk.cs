using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class RandomDotKinematogram : MonoBehaviour
{
    public GameObject dotPrefab; // Assign your dot prefab in the inspector
    public int numberOfDots = 100;
    public float stimulusRadius = 5f;
    public float dotSpeed = 1f;
    public float coherence = 0.5f; // 50% coherence
    private float previousCoherence = .5f; // Keep track of previous coherence
    public Vector3 coherenceDirection = Vector3.forward; // Work on changing this to a degree

    private GameObject[] dots;
    private bool[] isCoherent;

    void Start()
    {
        dots = new GameObject[numberOfDots];
        isCoherent = new bool[numberOfDots];
        
        // Determine the number of coherent dots
        int coherentCount = Mathf.RoundToInt(numberOfDots * coherence);
        
        for (int i = 0; i < numberOfDots; i++)
        {
            float radius = Random.Range(0f, stimulusRadius); // Radius within the circle
            float angle = Random.Range(0f, 2 * Mathf.PI); // Angle in radians

            // Convert polar coordinates to Cartesian for the X and Y positions
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            
            // Initialize Z position, can be set to 0 or any desired value/variation
            float z = 0; // Adjust as needed

            Vector3 randomPosition = new Vector3(x, y, z);
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);


            dots[i] = Instantiate(dotPrefab, randomPosition, Quaternion.identity, transform);
            
            // Assign coherence based on the predetermined number of coherent dots
            if (i < coherentCount)
            {
                isCoherent[i] = true; // This dot moves coherently
            }
            else
            {
                isCoherent[i] = false; // This dot moves randomly
            }

            // Attach the DotProperties component and set isCoherent
            DotProperties dotProps = dots[i].AddComponent<DotProperties>();
            dotProps.isCoherent = (i < coherentCount);
            dotProps.randomDirection =  randomDirection;

        }
        
        // Optional: Shuffle the isCoherent array if you want the coherent dots to be randomly distributed
        //Shuffle(isCoherent);
    }
    
    // Method to shuffle the boolean array
    private void Shuffle<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            T temp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    
    public void UpdateCoherence(float newCoherence)
    {
        // Calculate the number of dots that need their coherence updated
        int currentCoherentCount = Mathf.RoundToInt(numberOfDots * previousCoherence);
        int newCoherentCount = Mathf.RoundToInt(numberOfDots * newCoherence);
        int countToUpdate = Mathf.Abs(newCoherentCount - currentCoherentCount);

        if (newCoherence > previousCoherence)
        {
            // Increasing coherence: Make previously non-coherent dots coherent
            var nonCoherentDots = dots.Where(dot => !dot.GetComponent<DotProperties>().isCoherent).ToArray();
            Shuffle(nonCoherentDots); // Assuming your Shuffle method can handle GameObject[]

            for (int i = 0; i < countToUpdate && i < nonCoherentDots.Length; i++)
            {
                nonCoherentDots[i].GetComponent<DotProperties>().isCoherent = true;
            }
        }
        else if (newCoherence < previousCoherence)
        {
            // Decreasing coherence: Make previously coherent dots non-coherent
            var coherentDots = dots.Where(dot => dot.GetComponent<DotProperties>().isCoherent).ToArray();
            Shuffle(coherentDots); // Assuming your Shuffle method can handle GameObject[]

            for (int i = 0; i < countToUpdate && i < coherentDots.Length; i++)
            {
                coherentDots[i].GetComponent<DotProperties>().isCoherent = false;
            }
        }
        
        // Update the previous coherence level
        previousCoherence = newCoherence;
    }



    void Update()
    {
        // Update coherent motion as needed
        UpdateCoherence(coherence);

        foreach (GameObject dot in dots)
        {
            DotProperties dotProps = dot.GetComponent<DotProperties>();
            
            if (dotProps.isCoherent)
            {
                // Coherent movement
                dot.transform.position += coherenceDirection * dotSpeed * Time.deltaTime;
            }
            else
            {
                // Random movement
                dot.transform.position += dotProps.randomDirection * dotSpeed * Time.deltaTime;
            }

            // Recycle any dots that move past the stimulus boundaries

            // Check if the dot is out of bounds
            if (dot.transform.position.magnitude >= stimulusRadius)
            {
                    if (dotProps.isCoherent)
                {
                    // For coherent dots, move to previous position and invert coordinates
                    dot.transform.position -= coherenceDirection * dotSpeed * Time.deltaTime;
                    dot.transform.position *= -1f;
                }
                else
                {
                    // For random dots, move to previous position and invert coordinates
                    dot.transform.position -= dotProps.randomDirection * dotSpeed * Time.deltaTime;
                    dot.transform.position *= -1f;

                }
            }
        }
    }
}

