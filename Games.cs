using System.Security.Cryptography;

public enum RouletteBets {
	TopHalf = 37,
	BottomHalf = 38,

	TopThird = 39,
	MiddleThird = 40,
	BottomThird = 41,

	Red = 42,
	Black = 43,

	Even = 44,
	Odd = 45,

	LeftColumn = 46,
	MiddleColumn = 47,
	RightColumn = 48,
}


class Roulette {
	private enum RouletteColor {
		Green,
		Red,
		Black
	}

	public struct Game {
		public Dictionary<int, double> numberBets;
		public Dictionary<RouletteBets, double> specialBets;
		public int result;
		public double win;

		public Game(Dictionary<int, double> nb, Dictionary<RouletteBets, double> sb, int r, double w) {
			this.numberBets = nb;
			this.specialBets = sb;
			this.result = r;
			this.win = w;
		}
	}

	private Dictionary<int, double> numberBets;
	private Dictionary<RouletteBets, double> specialBets;
	private double totalBets;
	private double totalWin;
	List<Game> gameHistory;
	public int gamesPlayed = 0;

	public Roulette() {
		numberBets = new();
		for(int i = 0; i <= 36; i++) {
			numberBets.Add(i, 0);
		}
		specialBets = new();
		foreach(RouletteBets bet in Enum.GetValues(typeof(RouletteBets))) {
			specialBets.Add(bet, 0);
		}
		gameHistory = new();
		totalWin = 0;
	}

	public void PlaceNumberBet(int number, double betSize) {
		numberBets[number] += betSize;
		totalWin -= betSize;
		totalBets += betSize;
	}

	public void PlaceSpecialBet(RouletteBets bet, double betSize) {
		specialBets[bet] += betSize;
		totalWin -= betSize;
		totalBets += betSize;
	}

	private RouletteColor GetColor(int result) {
		if(result == 0) return RouletteColor.Green;
		else if(result % 2 == 0) return RouletteColor.Red;
		return RouletteColor.Black;
	}

	private double EvaluateResult(int result) {
		//Get Number Win
		double currentWin = 0.0;
		currentWin += numberBets[result] * 36;
		//Get Special Wins
		
		//Color & EvenOdd
		RouletteColor color = GetColor(result);	
		switch(color) {
			case RouletteColor.Red:
				currentWin += specialBets[RouletteBets.Red] * 2;
				currentWin += specialBets[RouletteBets.Even] * 2;
				break;
			case RouletteColor.Black:
				currentWin += specialBets[RouletteBets.Black] * 2;
				currentWin += specialBets[RouletteBets.Odd] * 2;
				break;
		}

		//Halves
		if(result > 0 && result <= 18) {
			currentWin += specialBets[RouletteBets.TopHalf] * 2;
		} else if(result > 18 && result <= 36) {
			currentWin += specialBets[RouletteBets.BottomHalf] * 2;
		}

		//Dozens
		if(result > 0 && result <= 12) {
			currentWin += specialBets[RouletteBets.TopThird] * 3;
		} else if(result > 12 && result <= 24) {
			currentWin += specialBets[RouletteBets.MiddleThird] * 3;
		} else if(result > 12 && result <= 24) {
			currentWin += specialBets[RouletteBets.BottomThird] * 3;
		}

		//Columns
		if(result != 0) {
			switch(result % 3) {
				case 0:
					currentWin += specialBets[RouletteBets.RightColumn] * 3;
					break;
				case 1:
					currentWin += specialBets[RouletteBets.LeftColumn] * 3;
					break;
				case 2:
					currentWin += specialBets[RouletteBets.MiddleColumn] * 3;
					break;
			}
		}

		totalWin += currentWin;
		return currentWin;
	}

	private int GetResult() {
		return RandomNumberGenerator.GetInt32(0, 37);
	}

	private void ResetBets() {
		for(int i = 0; i <= 36; i++) {
			numberBets[i] = 0;
		}
		var bets = Enum.GetValues(typeof(RouletteBets));
		foreach(RouletteBets bet in bets) {
			specialBets[bet] = 0;
		}
	}

	public double GetTotalBets() {
		return totalBets;
	}

	public double GetTotalBets(Dictionary<int, double> numberBets, Dictionary<RouletteBets, double> specialBets) {
		double bets = 0;
		for(int i = 0; i <= 36; i++) {
			bets += numberBets[i];
		}
		foreach(RouletteBets bet in Enum.GetValues(typeof(RouletteBets))) {
			bets += specialBets[bet];
		}
		return bets;
	}

	public static double GetTotalBets(Game game) {
		double bets = 0;
		for(int i = 0; i <= 36; i++) {
			bets += game.numberBets[i];
		}
		foreach(RouletteBets bet in Enum.GetValues(typeof(RouletteBets))) {
			bets += game.specialBets[bet];
		}
		return bets;
	}

	public double GetWinnings() {
		return totalWin;
	}

	public List<Game> GetGameHistory() {
		return gameHistory;
	}

	public void PrintStats() {
		Console.WriteLine($"Total Bets: {totalBets}");
		Console.WriteLine($"Total Pofit: {totalWin}");
		Console.WriteLine($"Games Played: {gamesPlayed}");
	}

	public static void PrintGame(Game game) {
		Console.WriteLine($"Total Bets: {GetTotalBets(game)}");
		Console.WriteLine($"Game Result: {game.result}");
		Console.WriteLine($"Game Winnings: {game.win}");
		Console.WriteLine($"Result: {GetTotalBets(game) + game.win}");
	}

	//Returns winnings of current Game
	public Game RunGame() {
		int result = GetResult();
		double balanceBefore = totalBets + totalWin;
		double currentWin = EvaluateResult(result);
		Dictionary<int, double> nb = new Dictionary<int, double>(numberBets);
		Dictionary<RouletteBets, double> sb = new Dictionary<RouletteBets, double>(specialBets);
		Game currentGame = new Game(nb, sb, result, currentWin - GetTotalBets(nb, sb));
		gameHistory.Add(currentGame);
		ResetBets();
		gamesPlayed++;
		return currentGame;
	}
}
