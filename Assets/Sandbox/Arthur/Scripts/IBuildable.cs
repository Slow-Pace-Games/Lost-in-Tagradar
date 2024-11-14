using UnityEngine;

public interface IBuildable
{
    // Check if the machine is buildable (collisions, resources)
    public bool CanBeBuilt(bool isSnapped);

    // Set the machine to placed and set the color of the material to white
    public void Build();

    // Rotate the machine
    public void Rotate(Quaternion rotation);

    //Reset the collision of the machine if the preview is too far to be drawn
    public void ResetCollisionCounter();
}