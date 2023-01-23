# asteroids-scriptable-objects
Tools Dev Assignment

The Editor Tool can be found in the Editor folder.
The ImprovedGameSettingsEditor is the final version and the GameSettingsEditor is the first iteration.
To open the tool in the Scene view, use menu Tools/Game Settings v2.


The first thing I had to do in this project was figure out how to work with ScriptableObjects and ScriptableEvents. This probably took up the majority of the first week of this project. Once I had some idea of where to start I started editing things in the project. At first I tried using the ScriptableObject variables that were already in the project but I soon realized that they were quite limited and would make our current assignment very hard to manage. So I decided to instead create my own settings ScriptableObject and derive three different classes from that one. I felt that I could keep the project both cleaner and more simple to manage this way.

After this I started working on the UI itself. I started doing this by finding what field I wanted to make editable and then by making the layout using UI Toolkit. Once a first draft was done I went over to Rider and started coding. 

In the end I have made two different scripts. The first one I made, called “GameSettingsEditor”, I have just saved to show the process but I never quite finished it. The second one, cleverly called “ImprovedGameSettingsEditor”, is the one that got finished and should be fully functional.

In the first one I started by hooking up to the different fields I created in the UI Toolkit by making methods and attaching them to callback. This works fine for a few fields but I soon realized that there was going to be way too much duplicated code, for my liking, if I continued that way. I also never got bindings to work very well using this method so I had to use callbacks two ways which probably would break down if the project were any larger. When I realized that I went on and started working on the second version.

With the second version I instead focused on working more through code. In this version I get all the Settings objects that exist in the project, of the types I want, and I create Foldouts for every one of them. I then go through all the fields in the Settings and call different methods depending on what kind of field they are. 

I have created different methods depending on what type of field I want to create. The CreateField method checks which kind of field is needed and uses one of the built in ones from UI Toolkit. The CreateCustomMinMaxSlider and CreateCustomMinMaxIntSlider both uses the build in field in the UI Toolkit to create and bind more custom fields where I felt like the built in ones were lacking or didn't exist at all as far as I could see. There are no Vector2Int fields in the Toolkit as far as I could find for example so I created my own, kind of. 

Finally I worked on the styling for the tool. I started by using the UI Toolkit window in Unity but I finished by looking at the documentation and writing the USS by hand as that felt easier as I have some prior experience with CSS.
