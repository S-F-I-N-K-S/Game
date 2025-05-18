using Microsoft.Xna.Framework;

namespace Race2;

public class Position
{
    
    public float X { get; set; }
    public float Y { get; set; }

    
    public Position(float x, float y)
    {
        X = x;
        Y = y;
    }
    
    public void Move(Vector2 direction, float speed)
    {
        X += direction.X * speed;
        Y += direction.Y * speed;
    }

    public void Move(Vector2 direction)
    {
        X += direction.X;
        Y += direction.Y;
    }
    
    public bool CheckCollision(Position other, int width, int height)
    {
        Rectangle myRect = new Rectangle((int)X, (int)Y, width, height);
        Rectangle otherRect = new Rectangle((int)other.X, (int)other.Y, width, height);
        return myRect.Intersects(otherRect); // [[4]]
    }
    
    public void ClampToScreen(int screenWidth, int screenHeight, int objectWidth, int objectHeight)
    {
        X = MathHelper.Clamp(X, 0, screenWidth - objectWidth);
        Y = MathHelper.Clamp(Y, 0, screenHeight - objectHeight);
    }
    public bool CheckTrackCollision(int[,] trackMap, int cellSize, int objectWidth, int objectHeight)
    {
        var leftCell = (X) / cellSize;
        var rightCell = (X + objectWidth - 1) / cellSize; 
        var topCell = (Y) / cellSize;
        var bottomCell = (Y + objectHeight - 1) / cellSize;
        
        for (var x = leftCell; x <= rightCell; x++)
        {
            for (var y = topCell; y <= bottomCell; y++)
            {
                if (x < 0 || y < 0 || y >= trackMap.GetLength(0) || x >= trackMap.GetLength(1))
                    return true; 
                
                if (trackMap[(int)(y), (int)(x)] == 1)
                    return true;
            }
        }
        return false;
    }
    public static Position operator +(Position position, Vector2 offset)
    {
        return new Position(position.X + offset.X, position.Y + offset.Y);
    }
}