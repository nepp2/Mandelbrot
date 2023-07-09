using Mandlebrot;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

var (width, height) = (1000, 800);

SimpleGame.Init(game => {
    game.Size = new Point(width, height);
    var graphics = game.Graphics;

    var shader = game.Content.Load<Effect>("mandelbrot");

    var tex = new Texture2D(graphics, width, height);
    tex.SetData(generateMandelbrot(width, height));

    bool spaceDown = false;

    void update(GameTime gameTime) {
        var keys = Keyboard.GetState();
        spaceDown = keys.IsKeyDown(Keys.Space);
    }

    var batch = new SpriteBatch(graphics);

    void draw(GameTime gameTime) {
        graphics.Clear(Color.Red);
        batch.Begin(effect: spaceDown ? null : shader);
        batch.Draw(tex, new Rectangle(0, 0, width, height), Color.White);
        batch.End();
    }

    return new(update, draw);
});

Color[] generateMandelbrot(int width, int height) {

    Vector2 complexSquare(Vector2 v) => new(
        x: v.X * v.X - v.Y * v.Y,
        y: 2 * v.X * v.Y
    );

    float compute(Vector2 coord) {
        const int iterations = 255;
        Vector2 z = new(0, 0);
        for (int i = 1; i < iterations; i++) {
            float sqrLen = z.X * z.X + z.Y * z.Y;
            if (sqrLen > 4)
                return i;
            z = complexSquare(z) + coord;
        }
        return iterations;
    }

    var data = new Color[width * height];
    var min = new Vector2(-2.2f, -1.2f);
    var max = new Vector2(1.2f, 1.2f);

    var realStep = (max.X - min.X) / width;
    var imagStep = (max.Y - min.Y) / height;

    for (int y = 0; y < height; y++) {
        var imag = min.Y + (imagStep * (y + 0.5f));
        for (int x = 0; x < width; x++) {
            var r = min.X + (realStep * x);
            var v = compute(new(r, imag));
            var grey = Math.Min(v / 40, 1);
            data[y * width + x] = new Color(grey, grey, grey);
        }
    }
    return data;
}
