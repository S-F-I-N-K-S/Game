﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Race2;

public class Game1 : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;

    private GameState currentGameState = GameState.MainMenu;
    
    private Texture2D carTexture;
    private Texture2D roadTexture;
    private Texture2D wallTexture;
    private Texture2D finishTexture;
    private SpriteFont font;
    private Button startButton;
    private Button level1Button;
    private Button level2Button;
    private Button level3Button;
    private Button car1Button;
    private Button exitButton;
    private Button educButton;
    private Button backButton;
    private MouseState mouseState;
    private Timer timer = new Timer();
    private string bestLap;
    private Vector2 cameraOffset;
    
    private PlayerCar playerCar;

    private int[,] trackMap;
    private double countdownTime = 3;
    private bool isCountdownActive = true;

    public List<GameState> history = new List<GameState>();
    
    /// <summary>
    /// Конструктор элементов класса Game1
    /// </summary>
    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        bestLap = "";
        graphics.PreferredBackBufferWidth = 800; // Начальная ширина
        graphics.PreferredBackBufferHeight = 600; // Начальная высота
        graphics.IsFullScreen = false; // Не полноэкранный режим
        graphics.ApplyChanges();
        Window.AllowUserResizing = true; // Разрешаем изменение размера окна
    }

    /// <summary>
    /// Метод, создающий окно
    /// </summary>
    protected override void Initialize()
    { 
        base.Initialize();
        Window.ClientSizeChanged += OnClientSizeChanged;
    }

    /// <summary>
    /// Метод, загружающий контент
    /// </summary>
    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        var startTexture = Content.Load<Texture2D>("start_button");
        var startHoverTexture = Content.Load<Texture2D>("start_button");
        var exitTexture = Content.Load<Texture2D>("exit_button");
        var exitHoverTexture = Content.Load<Texture2D>("exit_button");
        var educTexture = Content.Load<Texture2D>("educ_button");
        var educHoverTexture = Content.Load<Texture2D>("educ_button");
        var backTexture = Content.Load<Texture2D>("back_button");
        var backHoverTexture = Content.Load<Texture2D>("back_button");
        var level1Texture = Content.Load<Texture2D>("level1_button");
        var level1HoverTexture = Content.Load<Texture2D>("level1_button");
        var level2Texture = Content.Load<Texture2D>("level2_button");
        var level2HoverTexture = Content.Load<Texture2D>("level2_button");
        var level3Texture = Content.Load<Texture2D>("level3_button");
        var level3HoverTexture = Content.Load<Texture2D>("level3_button");
        var car1ButtonTexture = Content.Load<Texture2D>("car1_button");
        var car1HoverTexture = Content.Load<Texture2D>("car1_button");
        
        startButton = new Button(
            startTexture,
            startHoverTexture,
            new Rectangle(0, 0, 264, 72),
            StartGame
        );
        educButton = new Button(
            educTexture,
            educHoverTexture,
            new Rectangle(0, 0, 107, 20),
            Education
        );
        exitButton = new Button(
            exitTexture,
            exitHoverTexture,
            new Rectangle(0, 0, 98, 36 ),
            Exit
        );
        backButton = new Button(
            backTexture,
            backHoverTexture,
            new Rectangle(0, 0, 48, 18),
            Back
        );
        level1Button = new Button(
            level1Texture,
            level1HoverTexture,
            new Rectangle(0, 0, 100, 160),
            Level1
            );
        level2Button = new Button(
            level2Texture,
            level2HoverTexture,
            new Rectangle(0, 0, 100, 160),
            Level2
        );
        level3Button = new Button(
            level3Texture,
            level3HoverTexture,
            new Rectangle(0, 0, 100, 160),
            Level3
        );
        car1Button = new Button(
            car1ButtonTexture,
            car1HoverTexture,
            new Rectangle(0, 0, 80, 112),
            Car1
        );
        UpdateButtonPositions();
        carTexture = Content.Load<Texture2D>("car");
        roadTexture = Content.Load<Texture2D>("road");
        wallTexture = Content.Load<Texture2D>("wall");
        finishTexture = Content.Load<Texture2D>("finish");
        font = Content.Load<SpriteFont>("Arial");
    }

    /// <summary>
    /// Метод, обновляющий всю программу
    /// </summary>
    /// <param name="gameTime"></param>
    protected override void Update(GameTime gameTime)
    {
        mouseState = Mouse.GetState();
        if (currentGameState == GameState.MainMenu)
        {
            startButton.Update(mouseState);
            exitButton.Update(mouseState);
            educButton.Update(mouseState);
        }
        else if (currentGameState == GameState.Level)
        {
            level1Button.Update(mouseState);
            level2Button.Update(mouseState);
            level3Button.Update(mouseState);
            backButton.Update(mouseState);
        }
        else if (currentGameState == GameState.SelectCar)
        {
            car1Button.Update(mouseState);
            backButton.Update(mouseState);
        }
        else if (currentGameState == GameState.Educ)
        {
            backButton.Update(mouseState);
        }
        else if (currentGameState == GameState.Playing)
        {
            backButton.Update(mouseState);
            timer.Update(gameTime);
            timer.Start();
            if (isCountdownActive)
            {
                countdownTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (countdownTime <= 0)
                {
                    isCountdownActive = false; // Завершение отсчёта
                }
            }

            // Обновление игрока только после завершения отсчёта
            if (!isCountdownActive)
            {
                playerCar.Update(gameTime, trackMap, 64);
            }

            Rectangle playerBounds = new Rectangle(
                (int)playerCar.Position.X,
                (int)playerCar.Position.Y,
                playerCar.Texture.Width,
                playerCar.Texture.Height
                );

            for (int y = 0; y < trackMap.GetLength(0); y++)
            {
                for (int x = 0; x < trackMap.GetLength(1); x++)
                {
                    if (trackMap[y, x] == 2)
                    {
                        Rectangle finishBounds = new Rectangle(
                            x * 64,
                            y * 64,
                            64,
                            64
                            );

                        if (playerBounds.Intersects(finishBounds))
                        {
                            FinishGame();
                        }
                    }
                }
            }

            cameraOffset = new Vector2(
                -playerCar.Position.X + GraphicsDevice.Viewport.Width / 2,
                -playerCar.Position.Y + GraphicsDevice.Viewport.Height / 2
                );
        }
        base.Update(gameTime);
    }

    /// <summary>
    /// Метод, отрисовывающий всю программу
    /// </summary>
    /// <param name="gameTime"></param>
    protected override void Draw(GameTime gameTime)
    {
        spriteBatch.Begin();
            if (currentGameState == GameState.MainMenu)
            {
                startButton.Draw(spriteBatch);
                exitButton.Draw(spriteBatch);
                educButton.Draw(spriteBatch);
                GraphicsDevice.Clear(Color.White);
            }
            else if (currentGameState == GameState.Educ)
            {
                UpdateButtonPositions();
                string educName = "Управление:";
                Vector2 textSize = font.MeasureString(educName);
                var stringPosition = new Vector2(
                    Window.ClientBounds.Width / 2 - textSize.X / 2,
                    Window.ClientBounds.Height / 10
                );
                spriteBatch.DrawString(
                    font,
                    educName,
                    stringPosition,
                    Color.Black
                );
                string educ1 = "Газ - W";
                textSize = font.MeasureString(educ1);
                stringPosition = new Vector2(
                    Window.ClientBounds.Width / 2 - textSize.X / 2,
                    Window.ClientBounds.Height / 2 - textSize.Y - textSize.Y / 2
                );
                spriteBatch.DrawString(
                    font,
                    educ1,
                    stringPosition,
                    Color.Black
                );
                string educ2 = "Тормоз/задний ход - S";
                textSize = font.MeasureString(educ2);
                stringPosition = new Vector2(
                    Window.ClientBounds.Width / 2 - textSize.X / 2,
                    Window.ClientBounds.Height / 2 - textSize.Y / 2
                );
                spriteBatch.DrawString(
                    font,
                    educ2,
                    stringPosition,
                    Color.Black
                );
                string educ3 = "Поворот - A/D";
                textSize = font.MeasureString(educ3);
                stringPosition = new Vector2(
                    Window.ClientBounds.Width / 2 - textSize.X / 2,
                    Window.ClientBounds.Height / 2 + textSize.Y / 2
                );
                spriteBatch.DrawString(
                    font,
                    educ3,
                    stringPosition,
                    Color.Black
                );
                string educ4 = "Начать заново - R";
                textSize = font.MeasureString(educ4);
                stringPosition = new Vector2(
                    Window.ClientBounds.Width / 2 - textSize.X / 2,
                    Window.ClientBounds.Height / 2 + textSize.Y + textSize.Y / 2
                );
                spriteBatch.DrawString(
                    font,
                    educ4,
                    stringPosition,
                    Color.Black
                );
                backButton.Draw(spriteBatch);
                GraphicsDevice.Clear(Color.White);
            }
            else if (currentGameState == GameState.Level)
            {
                UpdateButtonPositions();
                backButton.Draw(spriteBatch);
                string selectLevel = "Выберите трассу:";
                Vector2 textSize = font.MeasureString(selectLevel);
                var stringPosition = new Vector2(
                    Window.ClientBounds.Width / 2 - textSize.X / 2,
                    Window.ClientBounds.Height / 10
                );
                spriteBatch.DrawString(
                    font,
                    selectLevel,
                    stringPosition,
                    Color.Black
                );
                level1Button.Draw(spriteBatch);
                level2Button.Draw(spriteBatch);
                level3Button.Draw(spriteBatch);
                GraphicsDevice.Clear(Color.White);
            }
            else if (currentGameState == GameState.SelectCar)
            {
                UpdateButtonPositions();
                backButton.Draw(spriteBatch);
                string selCar = "Выберите машину:";
                Vector2 textSize = font.MeasureString(selCar);
                var stringPosition = new Vector2(
                    Window.ClientBounds.Width / 2 - textSize.X / 2,
                    Window.ClientBounds.Height / 10
                );
                spriteBatch.DrawString(
                    font,
                    selCar,
                    stringPosition,
                    Color.Black
                );
                car1Button.Draw(spriteBatch);
                GraphicsDevice.Clear(Color.White);
            }
            else if (currentGameState == GameState.Playing)
            {
                UpdateButtonPositions();
                backButton.Draw(spriteBatch);
                GraphicsDevice.Clear(Color.Black);
                for (int y = 0; y < trackMap.GetLength(0); y++)
                {
                    for (int x = 0; x < trackMap.GetLength(1); x++)
                    {
                        Vector2 position = new Vector2(x * 64, y * 64) + cameraOffset;

                        if (trackMap[y, x] == 0)
                            spriteBatch.Draw(roadTexture, position, Color.White);
                        else if (trackMap[y, x] == 1)
                            spriteBatch.Draw(wallTexture, position, Color.White);
                        else if (trackMap[y, x] == 2)
                            spriteBatch.Draw(finishTexture, position, Color.White);
                    }
                    playerCar.Draw(spriteBatch, cameraOffset);
                    string formattedTime = timer.GetFormattedTime(timer.GetElapsedTime());
                    Vector2 textSize = font.MeasureString(formattedTime);
                    var timerPosition = new Vector2(
                        Window.ClientBounds.Width / 2 - textSize.X / 2,
                        50 // Фиксированное расстояние от верхнего края
                        );
                    spriteBatch.DrawString(
                        font,
                        formattedTime,
                        timerPosition,
                        Color.White
                        );
                    var bestPosition = new Vector2(
                        Window.ClientBounds.Width / 2 - textSize.X / 2,
                        50 - textSize.Y// Фиксированное расстояние от верхнего края
                        );
                    spriteBatch.DrawString(
                        font,
                        bestLap,
                        bestPosition,
                        Color.Yellow
                        );
                }
                if (isCountdownActive)
                {
                    string countdownText = Math.Ceiling(countdownTime).ToString();
                    Vector2 textSize = font.MeasureString(countdownText);
                    spriteBatch.DrawString(
                        font,
                        countdownText,
                        new Vector2(GraphicsDevice.Viewport.Width / 2 - textSize.X / 2, GraphicsDevice.Viewport.Height / 2 - textSize.Y / 2),
                        Color.White
                    );
                }
            }
            
            spriteBatch.End();
        
        base.Draw(gameTime);
    }

    /// <summary>
    /// Метод, ищущий стартовую позицию игрока
    /// </summary>
    /// <param name="trackMap">Карта уровня</param>
    /// <param name="cellSize">Размер клетки</param>
    /// <param name="carTexture">Текстура игрока</param>
    /// <returns>Элемент класса Vector2, описывающий стартовую позицию</returns>
    public static Vector2 FindSpawnPosition(int[,] trackMap, int cellSize, Texture2D carTexture)
        {
            for (int y = 0; y < trackMap.GetLength(0); y++)
            {
                for (int x = 0; x < trackMap.GetLength(1); x++)
                {
                    if (trackMap[y, x] == 2)
                    {
                        float baseX = x * cellSize + cellSize / 2;
                        float baseY = y * cellSize + cellSize / 2;

                        float offsetX = cellSize * 1.5f;
                        float offsetY = cellSize * 0.5f;

                        float spawnX = baseX + offsetX - carTexture.Width / 2;
                        float spawnY = baseY + offsetY - carTexture.Height / 2;

                        Position testPosition = new Position(spawnX, spawnY);
                        if (!testPosition.CheckTrackCollision(trackMap, cellSize, carTexture.Width,
                                carTexture.Height))
                        {
                            return new Vector2(spawnX, spawnY);
                        }
                    }
                }
            }

            return new Vector2(400, 300); 
        }
   
    /// <summary>
    /// Метод, описывающий логику кнопки старт
    /// </summary>
    private void StartGame()
    {
        history.Add(currentGameState);
        currentGameState = GameState.Level;
    }

    /// <summary>
    /// Метод, описывающий логику пересечения финишной черты
    /// </summary>
    private void FinishGame()
    {
        bestLap = timer.GetFormattedTime(timer.UpdateBestTime(timer.GetElapsedTime()));
        timer.Reset();
    }

    /// <summary>
    /// Метод, описывающий логику кнопки выбора трассы SSW
    /// </summary>
    private void Level1()
    {
        history.Add(currentGameState);
        trackMap = Levels.LevelsSelect(Level.SSW);
        currentGameState = GameState.SelectCar;
    }

    /// <summary>
    /// Метод, описывающий логику кнопки educ
    /// </summary>
    private void Education()
    {
        history.Add(currentGameState);
        currentGameState = GameState.Educ;
    }

    /// <summary>
    /// Метод, описывающий логику кнопки назад
    /// </summary>
    private void Back()
    {
        currentGameState = history[history.Count - 1];
        history.RemoveAt(history.Count - 1);
    }

    /// <summary>
    /// Метод, описывающий логику кнопки выбора трассы Monza
    /// </summary>
    private void Level2()
    {
        history.Add(currentGameState);
        trackMap = Levels.LevelsSelect(Level.Monza);
        currentGameState = GameState.SelectCar;
    }

    /// <summary>
    /// Метод, описывающий логику кнопки выбора трассы Monaco
    /// </summary>
    private void Level3()
    {
        history.Add(currentGameState);
        trackMap = Levels.LevelsSelect(Level.Monaco);
        currentGameState = GameState.SelectCar;
    }

    /// <summary>
    /// Метод, описывающий логику кнопки выбора машины 1
    /// </summary>
    private void Car1()
    {
        history.Add(currentGameState);
        Vector2 spawnPosition = FindSpawnPosition(trackMap, 64, carTexture);
        playerCar = new PlayerCar(carTexture, new Position(spawnPosition.X, spawnPosition.Y), 300f, 1.5f, 200f);
        currentGameState = GameState.Playing;
        countdownTime = 3f;
        isCountdownActive = true;
    }

    /// <summary>
    /// Метод, реализующий возможность изменения размеров окна
    /// </summary>
    /// <param name="sender">Отправитель</param>
    /// <param name="e"></param>
    private void OnClientSizeChanged(object sender, EventArgs e)
    {
        graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
        graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
        graphics.ApplyChanges();
        
        UpdateButtonPositions();
    }

    /// <summary>
    /// Метод, обновляющий позицию кнопок
    /// </summary>
    private void UpdateButtonPositions()
    {
        int screenWidth = Window.ClientBounds.Width;
        int screenHeight = Window.ClientBounds.Height;
        
        startButton.Rectangle = new Rectangle(
            screenWidth / 2 - startButton.Rectangle.Width / 2,
            screenHeight / 2 - 100, 
            startButton.Rectangle.Width,
            startButton.Rectangle.Height
        );

        exitButton.Rectangle = new Rectangle(
            screenWidth / 2 - exitButton.Rectangle.Width / 2,
            screenHeight / 2 + 50, 
            exitButton.Rectangle.Width,
            exitButton.Rectangle.Height
        );
        
        educButton.Rectangle = new Rectangle(
            screenWidth / 2 - educButton.Rectangle.Width / 2,
            screenHeight / 2, 
            exitButton.Rectangle.Width,
            exitButton.Rectangle.Height
        );
        if (currentGameState == GameState.SelectCar)
        {
            backButton.Rectangle = new Rectangle(
                screenWidth / 20 - backButton.Rectangle.Width / 2,
                screenHeight / 20 + backButton.Rectangle.Height, 
                backButton.Rectangle.Width,
                backButton.Rectangle.Height
            );
        }
        else
            backButton.Rectangle = new Rectangle(
                screenWidth / 20 - backButton.Rectangle.Width / 2,
                screenHeight / 20, 
                backButton.Rectangle.Width,
                backButton.Rectangle.Height
            );

        level1Button.Rectangle = new Rectangle(
            screenWidth / 2 - level1Button.Rectangle.Width * 2 ,
            screenHeight / 2 - level1Button.Rectangle.Height / 2 + 50,
            level1Button.Rectangle.Width,
            level1Button.Rectangle.Height
        );
        level2Button.Rectangle = new Rectangle(
            screenWidth / 2 - level2Button.Rectangle.Width / 2,
            screenHeight / 2 - level2Button.Rectangle.Height / 2 + 50,
            level2Button.Rectangle.Width,
            level2Button.Rectangle.Height
        );
        level3Button.Rectangle = new Rectangle(
            screenWidth / 2 + level3Button.Rectangle.Width,
            screenHeight / 2 - level3Button.Rectangle.Height / 2 + 50,
            level3Button.Rectangle.Width,
            level3Button.Rectangle.Height
        );
        car1Button.Rectangle = new Rectangle(
            screenWidth / 2 - car1Button.Rectangle.Width / 2 - car1Button.Rectangle.Width,
            screenHeight / 2 - car1Button.Rectangle.Height / 2 + 50,
            car1Button.Rectangle.Width,
            car1Button.Rectangle.Height
        );
    }
}