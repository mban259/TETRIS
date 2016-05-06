using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows;

class Teto {
    static int R;
    static int TurnPoint;
    const int SEARCHSTARTY = 19;
    const int SEARCHSTARTX = 3;
    static int BlockFlag = 0;
    static int[] BlockIndexX = { 0, 4, 8, 12 };
    static int[] BlockIndexY = { 0, 4, 8, 12, 16, 20, 24 };
    const int WIDTH = 16;
    static int GameOverFlag = 0;
    static ConsoleKey Input;
    static int[,] CollisionField = new int[HEIGHT, WIDTH];
    const int HEIGHT = 23;
    const int BWIDTH = 4;
    static int Fall, Side;
    const int BHEIGHT = BWIDTH;
    const int WAITTIME = 500;
    volatile static bool KeyRead = false;
    static Dictionary<int, char> B = new Dictionary<int, char>();
    static int[,] Stage = new int[HEIGHT, WIDTH];
    static int[,] Field = new int[HEIGHT, WIDTH];
    static void Main() {
        Start();
        MyInitVar();
        // ThreadPool.QueueUserWorkItem(new WaitCallback(MyHandler), null);
        while (true) {
            if (GameOverFlag == 0) {
                MyMakeBlock();
                MyGameOver();
                ThreadPool.QueueUserWorkItem(new WaitCallback(MyHandler), null);
                MyGetKey();
                InitInput();
                MyMakeField();
                MyInitField();
                MyFreezeBlock();
                MyDrawField();
                MyClearField();
                Thread.Sleep(WAITTIME);
            }
            else {
                Console.WriteLine("GameOver");
                break;
            }
            Fall++;
        }
    }
    static void Start() {
        B[0] = '　';
        B[1] = '□';
        B[9] = '■';
        B[2] = B[1];

    }
    static void MyMakeBlock() {
        Random rnd = new Random();
        int x, y;
        if (BlockFlag == 0) {
            R = rnd.Next(7);
            for (y = 0; y < BHEIGHT; y++) {
                for (x = 0; x < BWIDTH; x++) {
                    Block[y, x] = Blocks[BlockIndexY[R] + y, BlockIndexX[0] + x];
                }
            }
            BlockFlag = 1;
        }
    }
    static void MyMakeField() {
        int i, j, x, y;
        for (y = 0; y < BHEIGHT; y++) {
            for (x = 0; x < BWIDTH; x++) {
                Field[y + Fall, x + Side] = Block[y, x];
            }
        }
        for (i = 0; i < HEIGHT; i++) {
            for (j = 0; j < WIDTH; j++) {
                Field[i, j] += Stage[i, j];
            }
        }
    }
    static void MyInitVar() {
        Fall = 0;
        Side = 6;
        GameOverFlag = 0;
        BlockFlag = 0;
    }
    static void MyInitField() {
        int i, j;
        for (i = 0; i < HEIGHT; i++) {
            for (j = 0; j < WIDTH; j++) {
                Field[i, 0] = 9;
                Field[i, 1] = 9;
                Field[i, 2] = 9;
                Field[21, j] = 9;
                Field[22, j] = 9;
                Field[i, 14] = 9;
                Field[i,15] = 9;
                Field[20, j] = 9;
                Field[i, 13] = 9;
            }
        }
    }
    static void MyDrawField() {
        int i, j;
        Console.Clear();
        for (i = 0; i < HEIGHT-2; i++) {
            for (j = 2; j < WIDTH-2; j++) {
                Console.Write(B[Field[i, j]]);
            }
            Console.Write("\n");
        }
    }
    static void MyClearField() {
        int i, j;
        for (i = 0; i < HEIGHT; i++) {
            for (j = 0; j < WIDTH; j++) {
                Field[i, j] = 0;
            }
        }
    }
    static void MyMakeCollisitonField() {
        int i, j;
        for (i = 0; i < HEIGHT; i++) {
            for (j = 0; j < WIDTH; j++) {
                CollisionField[i, j] = Stage[i, j];
                CollisionField[i, 0] = 9;
                CollisionField[i, 1] = 9;
                CollisionField[i, 2] = 9;
                CollisionField[21, j] = 9;
                CollisionField[22, j] = 9;
                CollisionField[i, 14] = 9;
                CollisionField[i, 15] = 9;
                CollisionField[20, j] = 9;
                CollisionField[i, 13] = 9;
            }
        }
    }
    static void MyHandler(object userState) {
        Input = Console.ReadKey().Key;
        KeyRead = true;
        // Thread.Sleep(WAITTIME);
    }
    static void InitInput() {
        KeyRead = false;
    }
    static void MyGetKey() {
        int x, y;

        int SideFlag = 0;
        MyMakeCollisitonField();

        if (Input == ConsoleKey.LeftArrow && KeyRead) {

            for (y = 0; y < BHEIGHT; y++) {
                for (x = 0; x < BWIDTH; x++) {
                    if (Block[y, x] != 0) {
                        if (CollisionField[Fall + y, Side + x - 1] != 0) {
                            SideFlag++;
                        }
                    }
                }
            }
            if (SideFlag == 0) {
                Side--;
            }
        }
        if (Input == ConsoleKey.RightArrow && KeyRead) {

            for (y = 0; y < BHEIGHT; y++) {
                for (x = 0; x < BWIDTH; x++) {
                    if (Block[y, x] != 0) {
                        if (CollisionField[Fall + y, Side + x + 1] != 0) {
                            SideFlag++;
                        }
                    }
                }
            }
            if (SideFlag == 0) {
                Side++;
            }
        }
        if (Input == ConsoleKey.Z&&KeyRead) {
            MyTurnLeft();
        }
        if (Input == ConsoleKey.X&&KeyRead) {
            MyTurnRight();
        }
    }
    static void MyTurnRight() {
        int x, y;
        int turnFlag = 0;
        int[,] turnBlock = new int[4, 4];
        TurnPoint++;
        MyMakeCollisitonField();
        for (y = 0; y < BHEIGHT; y++) {
            for (x = 0; x < BWIDTH; x++) {
                turnBlock[y, x] = Blocks[BlockIndexY[R] + y, BlockIndexX[TurnPoint % 4] + x];
            }
        }
        for (y = 0; y < BHEIGHT; y++) {
            for (x = 0; x < BWIDTH; x++) {
                if (turnBlock[y, x] != 0) {
                    if (CollisionField[Fall + y, Side + x] != 0) {
                        turnFlag++;
                    }
                }
            }
        }
        if (turnFlag == 0) {
            for (y = 0; y < BHEIGHT; y++) {
                for (x = 0; x < BWIDTH; x++) {
                    Block[y, x] = turnBlock[y, x];
                }
            }
        }
        else {
            TurnPoint--;
        }
    }
    static void MyTurnLeft() {

    }
    static void MyFreezeBlock() {
        int x, y;
        int freezeFlag = 0;
        MyMakeCollisitonField();
        for (y = 0; y < BHEIGHT; y++) {
            for (x = 0; x < BWIDTH; x++) {
                if (Block[y, x] != 0) {
                    if (CollisionField[Fall + y + 1, Side + x] != 0) {
                        freezeFlag++;
                    }
                }
            }
        }
        if (freezeFlag != 0) {
            MySearchLine();
            MySaveField();
            MyInitVar2();
            
        }
    }
    static void MySaveField() {
        int i, j;
        for (i = 0; i < HEIGHT; i++) {
            for (j = 0; j < WIDTH; j++) {
                Stage[i, j] = Field[i, j];
            }
        }
    }
    static void MyInitVar2() {
        Fall = 0;
        Side = 6;
        BlockFlag = 0;
    }
    static void MyGameOver() {
        int x, y;
        MyMakeCollisitonField();
        for (y = 0; y < BHEIGHT; y++) {
            for (x = 0; x < BWIDTH; x++) {
                if (Block[y, x] != 0) {
                    if (CollisionField[Fall + y, Side + x] != 0) {
                        GameOverFlag++; 
                    }
                }
            }
        }
    }
    static void MySearchLine() {
        int i, j;
        int zeroCount = 0;
        int clearFlag = 0;
        int[] clearLinePoint = new int[4];
        int clearLineIndex = 0;
        int[] remainLinesPoint = new int[20];
        int remainLineIndex = 0;
        for (i = SEARCHSTARTY; i >= SEARCHSTARTY - 19; i--) {
            for (j = SEARCHSTARTX; j < SEARCHSTARTX + 10; j++) {
                if (Field[i, j] == 0) {
                    zeroCount++;
                }
            }
            if (zeroCount == 0) {
                clearFlag++;
                clearLinePoint[clearLineIndex] = i;
                clearLineIndex++;
            }
            else {
                remainLinesPoint[remainLineIndex] = i;
                remainLineIndex++;
                zeroCount = 0;
            }
        }
        if (clearFlag != 0) {
            for (clearLineIndex = 0; clearLineIndex < 4; clearLineIndex++) {
                if (clearLinePoint[clearLineIndex] != 0) {
                    for (j = SEARCHSTARTX; j < SEARCHSTARTX + 10; j++) {
                        Field[clearLinePoint[clearLineIndex], j] = 0;
                    }
                }
            }
            MyDrawField();
            Thread.Sleep(WAITTIME);
            remainLineIndex = 0;
            for (i = SEARCHSTARTY; i >= SEARCHSTARTY - 19; i--) {
                for (j = SEARCHSTARTX; j < SEARCHSTARTX + 10; j++) {
                    Field[i, j] = Field[remainLinesPoint[remainLineIndex], j];
                }
                remainLineIndex++;
            }
        }
    }
    static int[,] Block = new int[BHEIGHT, BWIDTH];
    static int[,] Blocks = {
    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
    {0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0},
    {0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0},
    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},

    {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0},
    {0,1,1,0,0,0,1,0,0,1,0,0,0,1,1,1},
    {0,0,1,0,1,1,1,0,0,1,1,0,0,1,0,0},
    {0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0},

    {0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0},
    {0,1,1,0,1,1,1,0,0,0,1,0,0,1,0,0},
    {0,1,0,0,0,0,1,0,0,1,1,0,0,1,1,1},
    {0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},

    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
    {0,0,1,0,1,1,0,0,0,0,1,0,1,1,0,0},
    {0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0},
    {0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0},

    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
    {0,1,0,0,0,0,1,1,0,1,0,0,0,0,1,1},
    {0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0},
    {0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0},

    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
    {0,1,0,0,0,0,0,0,0,1,0,0,0,1,0,0},
    {0,1,1,0,1,1,1,0,1,1,0,0,1,1,1,0},
    {0,1,0,0,0,1,0,0,0,1,0,0,0,0,0,0},

    {0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0},
    {0,1,0,0,1,1,1,1,0,1,0,0,1,1,1,1},
    {0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0},
    {0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0}
};
}
