# I Itch As I Crave
<div style="text-align: justify;">
    <p>
    A 2D game set in a 3D space where you will get to choose your dependency in life and follow the path of the one who cannot exist without it. Make sure to not lose the one who is guiding you.
    </p>
</div>

<button onclick="https://chronophobia.itch.io/i-itch-as-i-crave">Itch Page</button>


# Development
<div style="text-align: justify;">
    <p>
        In the following section, the main challenges and key points in the development process will be pointed out.
    </p>
</div>

## White Space

<div style="text-align: justify;">
    <p>
        The White space is actually a sphere that the player character can has to navigate on it and reach to a destination point which is TV/Monitor.
    </p>
</div>

### Sphere Gravity Replication
<div style="text-align: justify;">
<p>
To simulate walking on a sphere surface, at every frame, based on the input movement direction, we have to calculate using which vector we have to move the character so that on the current frame the local up vector of the character is perpendicular to the surface point (the tangent or bitangent of the surface at the current) or is parallel to the normal vector to the current point.
</p>
<p>
During the design process, we decided to limit the character movement on this Sphere to one main direction at the time, and basically skip the diagonal movements.
Therefore, unlike the usual situations where the combination of keyboard inputs can result in a simple vector addition, like:
</p>

<p>

```C#
velocity = character.forward * input.GetAxisRaw("Vertical") + character.right * input.GetAxisRaw("Horizontal");
```
</p>
<p>
It is important to exactly know which direction should the character go on the current frame to get to the next point, so instead, we capture and handle each key input separately to prevent the diagonal movement.
</p>
<p>
For this, we capture the keyboard imports in a Stack data structure, to keep the order of the key presses for the situations where multiple keys are pressed. A stack data structure follows the LIFO (Last-In-First-Out) principle, which means that the latest key input that was recorded will be the first one that will come out of the stack and will be applied to the movement direction calculation.
</p>
    
</div>

Capturing Inputs, storing them into the stack, and calling `Move()`:

https://github.com/smaooo/I_ITCH_AS_I_CRAVE/blob/c67d5014af6cea9b0e020f29398541b213229a2a/Assets/Scripts/WhiteRoomCharacter.cs#L68-L92

Also we should neutralize the order of inputs and always get rid of the same inputs that are behind the last one. This is for the situations that we have for example pressed `A`, then `W`, then `D` and then we have released `A` and pushed it again:

https://github.com/smaooo/I_ITCH_AS_I_CRAVE/blob/797d2ffe27027a9f990528fd411b1f60953d5a0e/Assets/Scripts/WhiteRoomCharacter.cs#L95-L168

Now, at every call to the `Move()`, we get the latest key input in the stack and based on the direction that the key input points to we set the velocity to either horizontally or vertically.

https://github.com/smaooo/I_ITCH_AS_I_CRAVE/blob/797d2ffe27027a9f990528fd411b1f60953d5a0e/Assets/Scripts/WhiteRoomCharacter.cs#L218-L252
