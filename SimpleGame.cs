
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mandlebrot;

public record struct GameLoop(Action<GameTime>? Update = null, Action<GameTime>? Draw = null);

// A wrapper around the game class so I don't have to extend it
public class SimpleGame {

    private Monogame game;

    public ContentManager Content { get => game.Content; }
    public GraphicsDevice Graphics { get => game.GraphicsDevice; }
    public Game MonoGame { get => game; }

    public Point Size {
        get => new(game.GraphicsManager.PreferredBackBufferWidth, game.GraphicsManager.PreferredBackBufferHeight);
        set {
            game.GraphicsManager.PreferredBackBufferWidth = value.X;
            game.GraphicsManager.PreferredBackBufferHeight = value.Y;
            game.GraphicsManager.ApplyChanges();
        }
    }

    private SimpleGame(Monogame game) {
        this.game = game;
    }

    public static void Init(Func<SimpleGame, GameLoop> init) {
        var game = new Monogame(init);
        game.Run();
    }

    private class Monogame : Game {

        Func<SimpleGame, GameLoop> init;
        GameLoop loop = new();
        SimpleGame game;

        public GraphicsDeviceManager GraphicsManager { get; }

        public Monogame(Func<SimpleGame, GameLoop> init) {
            GraphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.init = init;
            game = new(this);
        }

        protected override void Initialize() {
            init(game);
            base.Initialize();
        }

        protected override void LoadContent() {
            loop = init(new (this));
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime) {
            if (loop.Update is Action<GameTime> update)
                update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            if (loop.Draw is Action<GameTime> draw)
                draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
