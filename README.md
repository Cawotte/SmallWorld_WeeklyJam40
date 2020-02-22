# My first game jam project, Small Islands

The source code of my first Game jam project ! It's a game called "Small Islands" made 
in 4 days with Unity, during mid-april 2018, for the Weekly Game Jam #40 with the theme "Small World".

You can play the game by building it yourself in Unity, or just play it online on the <a href="https://cawotte.itch.io/small-islands">itchi.o page of the game</a>!

# The team
We were 3 people to work on the game :
- Me, Cawotte, I programmed the whole game, incorporated the assets, realized the game design and level design.
- Gardrek, an artist who did the pixel-art assets.
- Sunmachine, Sound designer, who made the music and SFX.

 # Big Refactor

 As of February 2020, I fully refactored the code of the game. 

 It means that the project is now compatible with Unity 2019.2 instead of 2017.3. It also means that the code is not the full mess it previously was. It had so much code gore I wonder how that repository managed to get 11 stars in the first place.

 Some notable changes in the refactor :

 - The Player class doesn't contain half of the code anymore.
 - Made a lot of new classes to split the responsabilities, principally codes moved away from the Player class.
 - Stopped using tags. Please, don't use tags. Use GetComponent<> instead.
 - New prefabs. Because the game was made before the huge prefab update, it had a lot of trouble with imbricked prefabs. Now all the scenes use the same prefabs.
 - The levels have been remade with the new prefabs, but are essentially the same.
 - Fixed some small bugs.
 - Use the new Unity's pixel perfect camera instead of an imported script.
 - Updated TextMeshPro to the in-Unity package instead of 2017's external plugin.
 - A bit of **ScriptableObject** based architecture!

#### ScriptableObject Based Archituecture

Singletons are bad. If you worked on any project longer than a few days, you might have started to use Singletons for global references to objects your prefab can't directly reference in the inspector. Like a GameManager, an AudioManager, an UIManager. If you did, you might have run into the probleme of Singletons initializing each other, turning it into a inter-dependant mess.

Turns out you can avoid most of this hassle by using ScriptableObjects for "global access", and keep the code modular. There can be also used for a lot of other useful purposes. I recommend to check that conference out, who explains it in great details  : https://www.youtube.com/watch?v=raQ3iHhE_Kk. 

My game is rather small, so my implementation of ScriptableObject architecture is quite small and not very impressive, but it's useful.


 # Other Credits
 Monogram Font (https://datagoblin.itch.io/monogram), a free font used in the game.

