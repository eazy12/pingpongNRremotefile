﻿using System;
using System.Windows.Forms;
using System.Collections;
using System.Timers;

namespace gamelogic
{
    public delegate void UpdateInfoEvent(UpdateInfo updateInfo);

    public class UpdateInfo : MarshalByRefObject
    {
        public UpdateInfo()
        {

        }
    }

    public class Ball : MarshalByRefObject
    {
        protected int x, y, width = 14, height = 16, speed = 2;
        protected bool goingLeft = false;
        protected bool goingTop = false;
        protected Game game;
        private Random r = new Random();
        public String status = "inactive"; // ENUM: inactive/active

        public Ball(Game g)
        {
            game = g;
            initLocation();
        }

        public void initLocation()
        {
            x = game.Width / 2;
            y = r.Next(10, game.Height - 10);

            goingLeft = Convert.ToBoolean(r.Next(0, 1));
            goingTop = Convert.ToBoolean(r.Next(0, 1));
        }

        public void changeOrientation()
        {
            goingLeft = !goingLeft;
            goingTop = Convert.ToBoolean(r.Next(0, 1));
        }

        public void setStatus(String newStatus)
        {
            status = newStatus;
        }

        public void move()
        {
            if (isGoingLeft)
            {
                x -= speed;
            }
            else
            {
                x += speed;
            }

            if (isGoingTop)
            {
                //y -= speed;
            }
            else
            {
                //y += speed;
            }
        }

        public bool isGoingLeft
        {
            get
            {
                return goingLeft;
            }
            set
            {
                goingLeft = value;
            }
        }

        public bool isGoingTop
        {
            get
            {
                return goingTop;
            }
            set
            {
                goingTop = value;
            }
        }

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }
    }

    public class Player : MarshalByRefObject
    {
        protected int x, y, width = 15, height = 70;
        protected Game game;
        protected int score = 0;

        public Player(Game g, int playerNum)
        {
            game = g;
            
            if (playerNum == 0)
            {
                x = 10;
            } else
            {
                x = g.Width - 40;
            }

            y = 0;
        }

        public void ChangePosition(String position)
        {
            if (position == "up")
            {
                Y = Y - game.speedSlide;

                if (Y < 0)
                {
                    Y = 0;
                }
            } else
            {
                Y = Y + game.speedSlide;

                if (Y  > game.AcitveHeight)
                {
                    Y = game.AcitveHeight;
                }
            }

            game.ChangePosition();
        }

        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
            }
        }

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }
    }

    public class Game : MarshalByRefObject
    {
        public event UpdateInfoEvent UpdateInfoHandle;
        protected UpdateInfo updateInfo = new UpdateInfo();
        protected ArrayList players = new ArrayList(); // 0 player - left side; 1 player = right side;
        protected Ball ball;
        protected int width = 450, height = 200, menuHeight = 70;
        public int speedSlide = 3;
        public String status = "init"; // ENUM: init/playing/finish

        public System.Timers.Timer TickTimer;

        public Game()
        {
            CreateBall();
            //Tick();

            TickTimer = new System.Timers.Timer(5);
            TickTimer.Elapsed += Tick; 
            TickTimer.AutoReset = true;
            TickTimer.Enabled = true;
        }

        public Player Connect() {
            if (players.Count > 1)
            {
                return null;
            }

            Player p = new Player(this, players.Count);
            players.Add(p);

            //Tick();
            return p;
        }

        public void DoUpdateInfo()
        {
            UpdateInfoHandle?.Invoke(updateInfo);
        }

        public void CreateBall()
        {
            ball = new Ball(this);
        }

        public void Disconnect(Player p) {
            players.Remove(p);
        }

        public void AddScore(Player p)
        {
            p.Score += 1;
        }

        public void ChangePosition()
        {
            //Tick();
        }

        public void SetStatus(String newStatus)
        {
            status = newStatus;

            if (status == "playing")
            {
                ball.setStatus("active");
            }
        }

        public void Tick(Object source, ElapsedEventArgs e)
        {
            if (status == "playing")
            {
                BallControl();
            }

            DoUpdateInfo();
        }

        public void BallControl()
        {
            if (ball.status != "active")
            {
                return;
            }

            if (ball.isGoingLeft)
            {
                if (CollisionLeft(ball))
                {
                    AddScore((Player)players[0]);
                    ball.initLocation();
                }
                if (!CollisionPlayer((Player)players[0]))
                {
                    ball.move();
                }
                else
                {
                    ball.changeOrientation();
                }
            }
            else
            {
                if (CollisionRight(ball))
                {
                    AddScore((Player)players[1]);
                    ball.initLocation();
                }
                if (!CollisionPlayer((Player)players[1]))
                {
                    ball.move();
                }
                else
                {
                    ball.changeOrientation();
                }
            }
        }

        public Boolean CollisionPlayer(Player player)
        {
            if ((Math.Abs(ball.X - player.X) < player.Width + ball.Width) &&
                (Math.Abs(ball.Y - player.Y) < player.Height + ball.Height))
            {
                return true;
            }
            return false;
        }

        public Boolean CollisionLeft(Ball _ball)
        {
            if (_ball.X <= 0)
            {
                return true;
            }
            return false;
        }

        public Boolean CollisionRight(Ball _ball)
        {
            if (_ball.X + ball.Width >= Width) 
            {
                return true;
            }
            return false;
        }

        public Boolean CollisionUp(Ball _ball)
        {
            if (_ball.Y <= 0)    
            {
                return true;
            }
            return false;
        }

        public Boolean CollisionDown(Ball _ball)
        {
            if (_ball.Y + ball.Height >= Height) 
            {
                return true;
            }
            return false;
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public int AcitveHeight
        {
            get
            {
                return height - menuHeight;
            }
        }

        public Ball Ball
        {
            get
            {
                return ball;
            }
        }

    }
}
