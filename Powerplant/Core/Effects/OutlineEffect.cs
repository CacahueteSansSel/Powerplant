namespace Powerplant.Core.Effects;

public class OutlineEffect : Effect
{
    int[] _ox = [-1, 1, 0, 0, -1, 1, -1, 1];
    int[] _oy = [0, 0, -1, 1, -1, -1, 1, 1];
    
    public bool IsEightNeighbor { get; set; }
    public bool IsSmooth { get; set; }
    public int Size { get; set; } = 1;
    public PwColor Color { get; set; } = PwColor.Black;

    public override bool Apply(ViewportBitmap referenceBitmap, ViewportBitmap targetBitmap)
    {
        ViewportBitmap bitmap = referenceBitmap;
        
        for (int i = 0; i < Size; i++)
        {
            DoOutline(bitmap, targetBitmap, i == Size-1);
            bitmap = targetBitmap.Copy();
        }

        return true;
    }

    void DoOutline(ViewportBitmap referenceBitmap, ViewportBitmap targetBitmap, bool mid)
    {
        for (int y = 0; y < referenceBitmap.Height; y++)
        {
            for (int x = 0; x < referenceBitmap.Width; x++)
            {
                if (referenceBitmap.Get(x, y).A == 0)
                    continue;

                for (int i = 0; i < (IsEightNeighbor ? 8 : 4); i++)
                {
                    bool isMid = IsSmooth && mid && i >= 4;
                    
                    int nx = x + _ox[i];
                    int ny = y + _oy[i];

                    if (nx < 0 || ny < 0 || nx >= referenceBitmap.Width || ny >= referenceBitmap.Height)
                        continue;

                    if (referenceBitmap.Get(nx, ny).A == 0 && targetBitmap.Get(nx, ny).A < 128)
                        targetBitmap.Set(nx, ny, isMid ? Color with {A = 64} : Color);
                }
            }
        }
    }
}