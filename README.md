# HBMF--Hard-Bullet-modding-framework-

A Hard Bullet mod to make modding easier, and to add some functions to the game.

# Resources

### Usage:
Start with `HBMF.r.`
“r” is short for “Resources”

### Example:
```cs
gameObject.transform.position = HBMF.r.playerloc;
```

### List of current resources:
- playerhandL/R 
- player
- playerloc
- playerHEAD
- Currentscene
# Inputs
- LeftJoystickClick  [Bool]
- RightJoystickClick  [Bool]
- LeftPrimButton  [Bool]
- RightPrimButton  [Bool]
- LeftMenuButton  [Bool]
- RightMenuButton  [Bool]
- LeftSecButton  [Bool]
- RightSecButton  [Bool]
- LeftTrackpad  [Bool]
- RightTrackPad  [Bool]
- LeftTriggerPress  [Bool]
- RightTriggerPress  [Bool]
- LeftVelocity  [Vector3]
- RightVelocity  [Vector3]
- LeftJoyAxis  [Vector2]
- RightJoyAxis  [Vector2]
- LeftGrip  [Float]
- RightGrip  [Float]
- LeftGripPress  [Bool]
- RightGripPress  [Bool]
## Example of input: 
```cs
if(HBMF.r.LeftJoystickClick) {

}
```

You’re going to want to stop this from repeating while the joystick is held.
Do the below:
```cs
  public override void OnUpdate()
    {
        if (HBMF.r.LeftJoystickClick)
        {
            if (loopstop == false)
            {
                loopstop = true;
                // Do whatever
            }
        }
        else
        {
            loopstop = false;
        }
    }

```
# MENU

To use the menu in your project, please make sure you add:
using BulletMenuVR;

At the top of your file.

To add a main page button, use the following code block in your onApplicationStart method in your MelonMod.
```cs
VrMenu.RegisterMainButton(new VrMenuButton("MyModButton", () =>
{
   // Anything in here will run when the button is pressed.
}
));
```

Although this is a really lame button, when it's pressed it wont open a page or anything, it'll just do one thing.

To make your button open a custom page which has buttons inside it, you need to first build a page using the VrMenuPageBuilder. You can make a VrMenuPageBuilder like so:

VrMenuPageBuilder builder = VrMenuPageBuilder.Builder();


With this, you can add buttons to the builder, using the addButton() method. The buttons are the same as the button you added originally.

### Example:
```cs
builder.AddButton(new VrMenuButton("First Button", () => 
{
    // Anything in here will be run when the button is pressed.
}, Color.red
));

builder.AddButton(new VrMenuButton("Second Button", () => 
{
    // Anything in here will be run when the button is pressed.
}, Color.orange
));
```

When you have completed adding all your buttons, you can store your page in a VrMenuPage object by calling the Build() method on the builder.

VrMenuPage myPage = builder.Build();


Now, we can run myPage.Open() in any button we want! You can have pages inside pages if you really wanted to, but for now we’ll just make our main button open the page.

Lets revisit the button we made originally and add that method inside it:
```cs
VrMenu.RegisterMainButton(new VrMenuButton("This is the label", () =>
{
   myPage.Open();
}, Color.yellow
));
```

Now when we press that button, it'll open our page we made! Here is a full class example of what something might look like:
```cs
using System;
using BulletMenuVR;
public class MyMod : MelonMod
{
    public override void OnApplicationStart(){
        VrMenuPageBuilder builder = VrMenuPageBuilder.Builder();
        
        builder.AddButton(new VrMenuButton("First Button", () => 
        {
            MelonLogger.Msg("First button was pressed!");
        }, Color.red
        )); 
        
        builder.AddButton(new VrMenuButton("Second Button", () => 
        {
            MelonLogger.Msg("Second button was pressed!");
        }, Color.orange
        ));
        
        VrMenuPage myPage = builder.Build();
        
        // This is the main button which will display on the main page of the menu.
        VrMenu.RegisterMainButton(new VrMenuButton("MyModButton", () =>
        {
            myPage.Open();
        }, Color.yellow
        ));
    }
} 
```

# Notifications

### Example:
```cs
// What the notification will say
HBMF.Notifications.notitext = "Something went wrong";

// How long it will appear for
HBMF.Notifications.notitime = 10f;

// Lastly to run the notification
HBMF.Notifications.NewNotification();
```

# AUDIO

To use the menu in your project, please make sure you add:
using AudioImporter;

At the top of your file.

### Example:
```cs
using AudioImporter;
public class MyMod : MelonMod
{
    AudioClip clip;

    public override void OnApplicationStart(){
        AudioAPI.Import(MelonUtils.UserDataDirectory + "\\MyMod\\sound.wav");
    }
    public override void OnSceneWasLoaded(int buildIndex, string sceneName){
        AudioAPI.CreateSource(new GameObject(), clip).Play();
    }
} 
```
This will play sound.wav inside of the MyMod folder inside of UserData at 0, 0, 0 when you load any scene.
