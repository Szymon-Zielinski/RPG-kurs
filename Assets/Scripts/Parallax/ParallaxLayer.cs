using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [SerializeField] private Transform background;
    [SerializeField] private float parallaxMultiplier;

    private float _imageFullWidth;
    private float _imageHalfWidth;

    public void CalcualteImageWidth()
    {
        _imageFullWidth = background.GetComponent<SpriteRenderer>().bounds.size.x;
        _imageHalfWidth = _imageFullWidth / 2;
    }
    public void Move(float distanceToMove)
    {
        background.position += Vector3.right * (distanceToMove * parallaxMultiplier); //new Vector3(distanceToMove * parallaxMultiplier, 0);
    }

    public void LoopBackground(float cameraLeftEdge, float cameraRightEdge)
    {
        float imageRightEdge = background.position.x + _imageHalfWidth;
        float imageLeftEdge = background.position.x - _imageHalfWidth;

        if (imageRightEdge < cameraLeftEdge)
            background.position += Vector3.right * _imageFullWidth;
        else if (imageLeftEdge > cameraRightEdge)
            background.position += Vector3.right * -_imageFullWidth;
            
    }
    

}
