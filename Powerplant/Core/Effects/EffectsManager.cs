using System.Collections.Generic;

namespace Powerplant.Core.Effects;

public static class EffectsManager
{
    private static List<Effect> _effects = [];

    public static Effect[] Effects => _effects.ToArray();

    public static void Init()
    {
        _effects =
        [
            new PureBlackEffect()
        ];
    }
}