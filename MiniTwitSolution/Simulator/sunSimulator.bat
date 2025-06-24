@echo off
echo Installing dependencies...
pip install --no-cache-dir -r requirements.txt

echo Running minitwit_simulator.py...
python minitwit_simulator.py "http://157.245.26.8:5002"

pause
