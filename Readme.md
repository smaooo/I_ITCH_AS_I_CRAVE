# I Itch As I Crave

<p style="text-align: justify;">
A 2D game set in a 3D space where you will get to choose your dependency in life and follow the path of the one who cannot exist without it. Make sure to not lose the one who is guiding you.
</p>

<button onclick="https://chronophobia.itch.io/i-itch-as-i-crave">Itch Page</button>

# Development
<p style="text-align: justify;">
    In the following section, the main challenges and key points in the development process will be pointed out.
</p>

## White Space

<p style="text-align: justify;">
    The White space is actually a sphere that the player character can has to navigate on it and reach to a destination point which is TV/Monitor.
</p>

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

https://github.com/smaooo/I_ITCH_AS_I_CRAVE/blob/4285e0a5a6af66c40b1b9a514be42326ee1f178c/Assets/Scripts/WhiteRoomCharacter.cs#L68-L92

<p style="text-align: justify;">
Also we should neutralize the order of inputs and always get rid of the same inputs that are behind the last one. This is for the situations that we have for example pressed `A`, then `W`, then `D` and then we have released `A` and pushed it again:
</p>

https://github.com/smaooo/I_ITCH_AS_I_CRAVE/blob/4285e0a5a6af66c40b1b9a514be42326ee1f178c/Assets/Scripts/WhiteRoomCharacter.cs#L95-L173

<p style="text-align: justify;">
Now, at every call to the `Move()`, we get the latest key input in the stack and based on the direction that the key input points to we set the velocity to either horizontally or vertically.
</p>

https://github.com/smaooo/I_ITCH_AS_I_CRAVE/blob/4285e0a5a6af66c40b1b9a514be42326ee1f178c/Assets/Scripts/WhiteRoomCharacter.cs#L218-L252

<p style="text-align: justify;">
With having filtered movement directions, now we can calculate the movement vector that results the character ending up on the sphere surface in the destination point.

Keep in mind that we have the raw movement vector, but we want to modify it in a way that the result of the applied velocity to the character, moves it in a way that the character stays on the sphere surface. For this:
1. We got the raw velocity (movement) vector by filtering out the key inputs.
2. We get the vector from the sphere center to the character which will give us the normal vector of the previous point.
3. We calculate the cross product of the raw velocity vector and the normal vector, which gives us a the tangent vector perpendicular vector to both.
4. Now, we get the vector from the sphere center to the position that the raw velocity results.
5. And finally, again, we calculate the cross product between the tangent vector and the sphere to raw new position vector to get the modified vector for movement.

Now that we have the corrected movement vector, we can calculate the final velocity by normalizing the tangent vector and then applying the raw velocity vector magnitude to it.
</p>

https://github.com/smaooo/I_ITCH_AS_I_CRAVE/blob/4285e0a5a6af66c40b1b9a514be42326ee1f178c/Assets/Scripts/WhiteRoomCharacter.cs#L254-L267


<img src="./.github/velocityModification.gif">


