# FireSimulator4911
COMP 4911 Fire Simulator Capstone Project
##Forest Fire Simulator:
A Unity-based simulation that models forest fire spread across a grid terrain using probabilistic ignition influenced by wind, fuel, temperature, humidity, and slope parameters.
****Features:****
Grid-based fire propagation model
Adjustable environmental parameters (wind, humidity, temperature)
Real-time visualization of fire spread
Interactive ignition point selection
Unit testing completed using Unity Testing Framework
****Running the project:****
1. Open in Unity 2022+
2. Load the main scene
3. Press play to start Unity scene
4. Adjust simulation parameters and click "Start Simulation"
****Controls: *****
Use "Q" and "E" to rotate the terrain left or right.
Click anywhere on the terrain to ignite a fire.
****Repository Structure:****
Assets/
    Materials/       *Unity Materials
    Prefabs/         *Unity Prefabs
    Scenes/          *Unity Scenes
    Scripts/	  *Simulation Logic
        Core/		      *Main logic controller
        Grid/		      *Grid creation and management
        SimMechanics/	*Burning cell probability
        UI/		        *All user interaction management
        UnitTests/	  *unit tests, run through "Test Runner"
