# Use an official Python runtime as a base image.
FROM python:3.9-slim

# Set the working directory inside the container.
WORKDIR /app

# Copy requirements file and install dependencies.
COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt

# Copy the rest of the simulator code into the container.
COPY . .

# Set an environment variable for the API host URL.
# In Docker Compose, this is overridden by SIMULATOR_HOST=http://api:80
ENV SIMULATOR_HOST="http://localhost:5000"

# Run the simulator, passing the API host URL as an argument.
#Uncomment to run the simulator on deployment
#CMD python minitwit_simulator.py "$SIMULATOR_HOST"