import cv2
import numpy as np
import pandas as pd

video = cv2.VideoCapture('registration_attempt/stimuli_cropped.mp4')
frame_count = int(video.get(cv2.CAP_PROP_FRAME_COUNT))
eye_track = pd.read_csv('registration_attempt/eyetracking.csv', sep=';')

# Clean up NaN trials at end of file
eye_track = eye_track.dropna(subset=['TrialNumber'])
eye_track['TrialNumber'] = eye_track['TrialNumber'].astype(int)

gaze_data = pd.DataFrame()
gaze_data[['x', 'y']] = eye_track['CombinedGazeForward'].str.extract(r'\(([^,]+), ([^,]+), [^)]+\)').astype(float)
gaze_data[['TrialNumber', 'CaptureTime', 'HMDRotation', 'Frame']] = eye_track[['TrialNumber', 'CaptureTime', 'HMDRotation', 'Frame']]
gaze_data['Frame'] = gaze_data['Frame'] - np.min(gaze_data['Frame'])
gaze_data = gaze_data.loc[gaze_data['TrialNumber'] < 46]

print(gaze_data.iloc[-5:,:])
print(len(gaze_data))
print(frame_count)

print(f"x-min: {np.min(gaze_data['x'])}, x-max:{np.max(gaze_data['x'])}")
print(f"y-min: {np.min(gaze_data['y'])}, y-max:{np.max(gaze_data['y'])}")

xy_data = pd.DataFrame()
xy_data[['x', 'y']] = gaze_data[['x', 'y']]

def remap_spatial(value, old_min=-1, old_max=1, new_min=0, new_max=512):
    # Ensure the value is within the old range
    value = max(old_min, min(value, old_max))
    # Calculate the proportional position of the value in the old range
    old_range = old_max - old_min
    new_range = new_max - new_min
    scaled_value = (value - old_min) / old_range
    # Map this position to the new range
    return new_min + (scaled_value * new_range)

def resample_dataframe(df, original_freq=200, target_freq=60):
    # Ensure the DataFrame has a datetime index
    if not isinstance(df.index, pd.DatetimeIndex):
        # Create a datetime index based on the original frequency
        df.index = pd.date_range(start='2000-01-01', 
                                 periods=len(df), 
                                 freq=f'{1000000/original_freq}U')  # microseconds
    
    # Calculate the target frequency in microseconds
    target_period_micros = int(1000000 / target_freq)
    target_freq_str = f'{target_period_micros}us'
    
    # Resample the DataFrame
    resampled_df = df.resample(target_freq_str).mean()
    
    return resampled_df

xy_data = resample_dataframe(xy_data, original_freq=200, target_freq=60)

xy_data['x'] = [int(remap_spatial(x)) for x in xy_data['x']]
xy_data['y'] = [512-int(remap_spatial(y)) for y in xy_data['y']]
print(f"x-min: {np.min(gaze_data['x'])}, x-max:{np.max(gaze_data['x'])}")
print(f"y-min: {np.min(gaze_data['y'])}, y-max:{np.max(gaze_data['y'])}")
print(xy_data)


print(512-remap_spatial(-1))

ii = 0

while True:
    ret, frame = video.read()

    cv2.circle(frame, (xy_data['x'][ii], xy_data['y'][ii]), 5, (255,0,0), -1)

    cv2.imshow('video', frame)
    
    ii += 1

    if cv2.waitKey(16) == ord('q'):
        break