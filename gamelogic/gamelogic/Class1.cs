﻿using System;
using System.Windows.Forms;
using System.Collections;
using System.Timers;

namespace gamelogic
{
    public class Ball : MarshalByRefObject
    {
        protected int x, y, width = 14, height = 16, speedY = 2, speedX = 2;
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
                x -= speedX;
            }
            else
            {
                x += speedX;
            }

            if (isGoingTop)
            {
                y -= speedY;
            }
            else
            {
                y += speedY;
            }
        }
        public int SpeedY
        {
            get
            {
                return speedY;
            }
            set
            {
                speedY = value;
            }
        }

        public int SpeedX
        {
            get
            {
                return speedX;
            }
            set
            {
                speedX = value;
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
                x = 0;
            } else
            {
                x = g.Width - width;
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

                if (Y  > game.AcitveHeight - Height)
                {
                    Y = game.AcitveHeight - Height;
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
        public delegate void UpdateInfoHandler(object sender, EventArgs e);
        protected ArrayList players = new ArrayList(); // 0 player - left side; 1 player = right side;
        protected Ball ball;
        protected int width = 427, height = 241;
        public int speedSlide = 5;
        public String status = "init"; // ENUM: init/playing/finish/waiting2player

        public System.Timers.Timer TickTimer;

        protected Action callback;

        public Game()
        {
            Console.WriteLine("I am created");
            CreateBall();

            TickTimer = new System.Timers.Timer(5);
            TickTimer.Elapsed += Tick; 
            TickTimer.AutoReset = true;
            TickTimer.Enabled = true;
        }

        ~Game()
        {
            Console.WriteLine("Destroyed");
        }

        public Player Connect() {
            Console.WriteLine("Connect is called");

            if (players.Count > 2)
            {
                return null;
            }
            else if (players.Count == 1)
            {
                SetStatus("waiting2player");
            }
            else if (players.Count == 2)
            {
                SetStatus("playing");
            }

            Player p = new Player(this, players.Count);
            players.Add(p);

            return p;
        }

        public Player getPlayer(int index)
        {
            if (players.Count > index)
            {
                return (Player)players[index];
            }
            else
            {
                return null;
            }
        }

        public void setCallback(Action cb)
        {
            callback = cb;
        }

        public void DoUpdateInfo()
        {
            callback?.Invoke();
        }

        public void CreateBall()
        {
            ball = new Ball(this);
        }

        public void Disconnect(Player p) {
            try
            {
                players.Remove(p);
                Console.WriteLine(players.Count);
                if (players.Count == 0)
                {
                    
                }
            }
            catch { Console.WriteLine("catched"); }
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
            else if (status == "finish")
            {
                ball.setStatus("inactive");
            }
            else if ( status == "waiting2player")
            {
                ball.setStatus("inactive");
            }
        }

        public void Tick(Object source, ElapsedEventArgs e)
        {
            if (status == "playing")
            {
                BallControl();
            }
            else if (status == "finish")
            {
                TickTimer.Enabled = false;
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
                try
                {
                    if (CollisionLeft(ball))
                    {
                        AddScore((Player)players[0]);
                        if( getPlayer(0).Score > 2)
                        {
                            status = "finish";
                        }
                        ball.initLocation();
                    }
                    if (CollisionDown(ball))
                    {
                        ball.isGoingTop = true;
                    }
                    if (CollisionUp(ball))
                    {
                        ball.isGoingTop = false;
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
                catch
                {
                    Disconnect((Player)players[0]);
                }
            }
            else
            {
                try
                {
                    if (CollisionRight(ball))
                    {
                        AddScore((Player)players[1]);
                        if (getPlayer(1).Score > 2)
                        {
                            status = "finish";
                        }
                        ball.initLocation();
                    }
                    if (CollisionUp(ball))
                    {
                        ball.isGoingTop = false;
                    }
                    if (CollisionDown(ball))
                    {
                        ball.isGoingTop = true;
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
                catch
                {
                    if( players.Count > 1)
                    Disconnect((Player)players[1]);
                }
                
            }
           
        }

        public Boolean CollisionPlayer(Player player)
        {
            if ((Math.Abs(ball.X - player.X) <= player.Width) &&
                ( (player.Y - ball.Height < ball.Y) && (( player.Y + player.Height ) >= ball.Y)))
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
            if (_ball.X + ball.Width/2 >= Width) 
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
            if (_ball.Y + ball.Height >= AcitveHeight) 
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
                return height;
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
