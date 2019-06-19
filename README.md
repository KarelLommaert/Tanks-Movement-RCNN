# Tanks-Movement-RCNN
Setting up the project:
1) Download Unity's ML-agents project from their github: https://github.com/Unity-Technologies/ml-agents

2) Install all necessary libraries and software. Detailed guide available at https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Installation.md

3) Open the tanks game and add the ML-agents toolkit (detailed instructions can also be found on Unity's github)

4) Import the ML-agents/Plugins from the toolkit to the Tanks game\Assets\ML-Agents folder

5) The project should now compile. You can press play to start a game session. At the end of the session, all spatio-temporal data will be collected and converted automatically. It gets saved to a subfolder called "SavedSequences" in the TanksGame folder

6) Adjust the path to the SavedSequences folder in the neural networks, and you're ready to train

Note: Alternatively, you can download the executable of the game on http://karellommaert.com/movementneuralnetwork.html
This version also saves your spatio-temporal data, and is more accessible if you just want to test the network.
