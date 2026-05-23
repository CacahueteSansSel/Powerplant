using Powerplant.Controls;
using Powerplant.Core.Effects;

namespace Powerplant.Core;

public class EffectRunner<T> where T : Effect, new()
{
    private Effect _effect;
    private ViewportBitmap _originalBitmapCopy;
    private ViewportControl _viewport;

    public Command RunEffectCommand => new((T)_effect, _originalBitmapCopy);
    public T Effect => (T)_effect;

    public EffectRunner(ViewportControl viewport)
    {
        _effect = new T();
        _viewport = viewport;

        _originalBitmapCopy = _viewport.Bitmap.Copy();
    }

    public bool Apply()
    {
        ViewportBitmap newBitmap = _originalBitmapCopy.Copy();
        
        if (!_effect.Apply(_originalBitmapCopy, newBitmap)) 
            return false;
        
        _viewport.SetBitmap(newBitmap);

        return true;
    }

    public void Reset()
    {
        _viewport.SetBitmap(_originalBitmapCopy);
    }

    public class Command : Commands.Command
    {
        private T _effect;
        private ViewportBitmap _oldBitmap;

        public Command(T effect, ViewportBitmap oldBitmap)
        {
            _oldBitmap = oldBitmap;
            _effect = effect;
        }
        
        public override void Run()
        {
            ViewportBitmap newBitmap = _oldBitmap.Copy();
            if (!_effect.Apply(_oldBitmap, newBitmap)) return;
            
            Viewport.SetBitmap(newBitmap);
        }

        public override void Undo()
        {
            Viewport.SetBitmap(_oldBitmap);
        }
    }
}