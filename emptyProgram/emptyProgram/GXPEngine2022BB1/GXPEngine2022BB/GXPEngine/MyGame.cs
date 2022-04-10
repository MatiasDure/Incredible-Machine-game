using System;									// System contains a lot of default C# libraries 
using System.Collections.Generic;
using GXPEngine;                                // GXPEngine contains the engine

public class MyGame : Game
{
	List<Tank> tanks;
	List<NLineSegment> lines;
	List<Ball> points;

	Ball currentBall;
	Flag flag;

	EasyDraw levelStatus;
	EasyDraw title;
	EasyDraw tutorial;

	int currentLevel = 1;
	int amountLevels = 4;

	bool levelIterationStarted = false;
	bool beatLevel = false;
	
	public MyGame() : base(1280, 720, false)		// Create a window that's 800x600 and NOT fullscreen
	{
		tanks = new List<Tank>();
		lines = new List<NLineSegment>();
		points = new List<Ball>();

		LoadLevel(currentLevel);

		Vector2.PerformUnitTest(); //unit test for Vec2
	}

	// For every game object, Update is called every frame, by the engine:
	void Update()
	{
		UpdateText(levelStatus, String.Format("Level succeded: {0}!",beatLevel));
		HandleInput();
		if (!levelIterationStarted && Input.GetKeyDown(Key.ENTER))
		{
			levelIterationStarted = true;
			for (int i = 0; i < tanks.Count; i++) tanks[i].IterationStarted();
		}

		if (!beatLevel && levelIterationStarted) LevelIteration();
	}

	static void Main()
	{
		Game myGame = new MyGame();
		myGame.Start();
	}

	//starts the iteration of the level
	void LevelIteration()
    {
		currentBall.Step();
		
		for (int i = 0; i < tanks.Count; i++)
        {
			if (currentBall != null)
			{
				tanks[i].GetBall(currentBall);
			}
			currentBall = tanks[i].Shooting();
		}
		flag.GetBall(currentBall);
		beatLevel = flag.SetLevelStatus();
		CheckBoundary();
	}

	//Used to change between scenes
	void HandleInput()
    {
		targetFps = 5;
		for(int i = 0; i < amountLevels;i++)
        {
			if (Input.GetKeyDown(i + 48)) LoadLevel(i);
		}	
    }

	void CheckBoundary()
    {
		if (currentBall.Position.x + currentBall.radius > width ||
			currentBall.Position.x - currentBall.radius < 0 ||
			currentBall.Position.y + currentBall.radius > height) LoadLevel(currentLevel);
    }

	//Loads level to display on screen
	void LoadLevel(int pSceneToLoad)
    {
		ResetLevel();
		currentLevel = pSceneToLoad;

		switch(pSceneToLoad)
        {
			case 1:
				//tank 1
				AddTank(new Vector2(700, height / 2));

				//line 1
				AddLine(new Vector2(50, 100), new Vector2(600, 250));

				//starting ball 
				currentBall = new Ball(20, new Vector2(100, 70), new Vector2(0, 0), new Vector2(0, 0.1f));

				//finish flag
				flag = new Flag(new Vector2(400, 500));
				break;

			case 2:
				//tank 1
				AddTank(new Vector2(800, 250));

				//line 1
				AddPolygon(new Vector2[] { new Vector2(650, 600), new Vector2(650, 180), new Vector2(200, 180) });
				AddLine(new Vector2(40, 100), new Vector2(450, 500));

				//starting ball 
				currentBall = new Ball(20, new Vector2(800, 70), new Vector2(0, 0), new Vector2(0, 0.1f));

				//finish flag
				flag = new Flag(new Vector2(550, 600));
				break;

			case 3:

				break;

			case 4: 

				break;

			default: //Create your own level! :D Use AddPolygon or/and AddLine

				//starting ball 
				currentBall = new Ball(20, new Vector2(100, 70), new Vector2(0, 0), new Vector2(0, 0.1f));

				//finish flag
				flag = new Flag(new Vector2(400, 500));
				break;
        }

		LoadText();
        try
        {
			AddChild(currentBall);
			AddChild(flag);
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
		
	}

	void LoadText()
    {
		title = new EasyDraw(280, 60, false);
		title.SetXY(width / 2 - title.width / 2, 20);
        UpdateText(title, "The Incredible Machine V -2.0");
        
		levelStatus = new EasyDraw(250, 60, false);
		levelStatus.SetXY(0, 20);

		tutorial = new EasyDraw(300, 120, false);
		tutorial.SetXY(width - tutorial.width, 20);
		UpdateText(tutorial, "Tutorial:\n1. Left click to select tank" +
								"\n2. Press ENTER to start iteration" +
								"\n3. Switch level by pressing 1-4" + 
								"\n4. Reach the blue flag to win!");

		AddChild(levelStatus);
		AddChild(title);
		AddChild(tutorial);
	}

	void UpdateText(EasyDraw obj, string pText)
    {
		obj.ClearTransparent();
		obj.TextAlign(CenterMode.Center, CenterMode.Min);
		obj.Fill(255);
		obj.Text(pText);
	}

	//Removes all objects from the game
	void RemoveObjects()
    {
		List<GameObject> children = game.GetChildren();
		foreach(GameObject obj in children)
        {
			obj.Destroy();
        }
	}

	//Resets level
	void ResetLevel()
    {
		RemoveObjects();
		lines.Clear();
		tanks.Clear();
		levelIterationStarted = beatLevel = false;
    }

	//Adds a polygon shape
	void AddPolygon(Vector2[] pPoints, int pRadius = 0)
    {
		if (pPoints.Length < 2) return;
		for(int i = pPoints.Length - 1; i > 0; i--) //going through array in reverse to avoid index out of bound exception
        {
			NLineSegment line = new NLineSegment(pPoints[i-1],pPoints[i], 0xff00ff00, 4); //start point would be 
			AddChild(line);
			lines.Add(line);
        }
		NLineSegment closeLine = new NLineSegment(pPoints[pPoints.Length - 1], pPoints[0], 0xff00ff00, 4); //close opening by creating a line between first and last points
		AddChild(closeLine);
		lines.Add(closeLine);
		AddLineCaps(pPoints, pRadius);
    }

	//Adds line caps to lines
	void AddLineCaps(Vector2[] pPoints, int pRadius = 0)
    {
		for(int i = 0; i < pPoints.Length; i++)
        {
			Ball lineCap = new Ball(pRadius, pPoints[i], new Vector2(), new Vector2(), false);
			AddChild(lineCap);
			points.Add(lineCap);
        }
    }

	//Adds line
	void AddLine(Vector2 pStart, Vector2 pEnd, int pRadius = 0)
	{
		for(int i = 0; i < 2; i++)
        {
			NLineSegment line = new NLineSegment(pStart, pEnd, 0xff00ff00, 4);
			AddChild(line);
			lines.Add(line);
			if (i > 0) break;
			Vector2 holder = pStart;
			pStart = pEnd;
			pEnd = holder;
		}
		AddLineCaps(new Vector2[] { pStart, pEnd }, pRadius);
	}

	//Adds tank to list and game
	void AddTank(Vector2 pPosition)
    {
		Tank tank = new Tank(pPosition);
		AddChild(tank);
		tanks.Add(tank);
    }

	//returns list count
	public int GetLinesCount() => lines.Count;
	public int GetPointsCount() => points.Count;

	//returns object at specified index from list
	public Ball GetPointAtIndex(int pIndex) => points[pIndex];
	public NLineSegment GetLineAtIndex(int pIndex) => lines[pIndex];

}