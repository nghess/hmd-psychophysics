import plotly.express as px
import plotly.graph_objects as go
from scipy.stats import gaussian_kde
from sklearn.linear_model import LinearRegression
from sklearn.preprocessing import PolynomialFeatures
import seaborn as sns
import matplotlib.pyplot as plt
import pandas as pd

def linear_regression(data:pd.core.frame.DataFrame, dv:str, iv:str, filter:int=0, binary_response:bool=True, title:str=''):

    # Predict correct trials by reaction time
    df = data

    # Filter outliers
    if filter != 0:
        df = df[df[iv] <= filter]

    # Convert 'Response' column to numeric (True -> 1, False -> 0)
    if binary_response:
        df[dv] = df[dv].astype(int)

    # Define the iv and target variables
    X = df[[iv]]
    y = df[dv]

    # Perform linear regression
    model = LinearRegression()
    model.fit(X, y)

    # Predict values
    df['Predicted'] = model.predict(X)

    # Visualization with Plotly
    fig = px.scatter(df, x=iv, y=dv, title=title)
    fig.add_trace(go.Scatter(x=df[iv], y=df['Predicted'], mode='lines', name='Regression Line'))

    # Show plot
    fig.show()