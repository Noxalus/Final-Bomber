using System;
using Microsoft.Xna.Framework;

namespace FBClient.Core
{
    interface IDrawableGameComponent : IGameComponent, IUpdateable, IDisposable, IDrawable
    {
    }
}
