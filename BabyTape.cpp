#include <fstream>
#include <cmath>

#define		FILE_NAME_INPUT "input.txt"
#define		FILE_NAME_OUTPUT "output.txt"


using namespace std;

struct Score_st
{
	int IdealScore;
	int UnOptimalScore;
};


//GLOBAL--------------------------------------
///pair<int,int> : first - ideal score; second - unoptimal score;
Score_st ScoreField[1001][1001];
int TapeSize, SummaryScore[1001], Tape[1001];
//--------------------------------------------


void Read_Data()
{
	ifstream input(FILE_NAME_INPUT);

	input >> TapeSize;
	for (int i = 0; i < TapeSize; i++)
	{
		input >> Tape[i];
	}
	SummaryScore[0] = Tape[0];
	for (int i = 1; i < TapeSize; i++)
	{
		SummaryScore[i] = SummaryScore[i - 1] + Tape[i];
	}

	input.close();
}


void Set_Dp_Base()
{
	for (int i = 0; i < TapeSize; i++)
	{
		ScoreField[i][i].UnOptimalScore = 0;
		ScoreField[i][i].IdealScore = Tape[i];
	}
}


int Other_Points(const int _i,const int _j, const int _CurrentPoints)
{
	int ScoreBefore_i = _i > 0 ? SummaryScore[_i - 1] : 0;

	return SummaryScore[_j] - ScoreBefore_i - _CurrentPoints;
}


void Dp_Work_Out() 
{
	if (TapeSize > 1)
	{
		int CurrentColumn = 1, i, j, Limit;
		while (CurrentColumn < TapeSize)
		{
			i = 0; j = CurrentColumn;
			Limit = TapeSize - CurrentColumn;
			while (i < Limit)
			{
				int LeftTaken = Tape[i] + ScoreField[i + 1][j].UnOptimalScore,
					RightTaken = Tape[j] + ScoreField[i][j - 1].UnOptimalScore;
				ScoreField[i][j].IdealScore = fmax(LeftTaken, RightTaken);
				ScoreField[i][j].UnOptimalScore = Other_Points(i, j, ScoreField[i][j].IdealScore);
				
				++i; ++j;
			}

			++CurrentColumn;
		}
	}
}


void Print_Answer()
{
	ofstream output(FILE_NAME_OUTPUT);

	output << ScoreField[0][TapeSize - 1].IdealScore;

	output.close();
}


int main()
{
	Read_Data();

	Set_Dp_Base();

	Dp_Work_Out();

	Print_Answer();

	return 0;
}