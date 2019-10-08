#include <fstream>
#include <set>
#include <cmath>
#include <vector>

#define QT_JUMPS 8
#define ID_BLACK 1
#define ID_WHITE -1
#define ID_EMPTY 0
#define PRICE_EMPTY 1
#define PRICE_BLACK 2
#define ANSWER_NO "No"




using namespace std;


//GLOBAL---------------------
set < pair < int, pair< int,int > > > set_ToVisit;
short int ChessFieldVal[1002][1002];
pair<int, int> Jump[8];
int X1, Y1, X2, Y2, N, M, BeginNum, DistanceTo[1002][1002];
//---------------------------

bool Suitable(int coordinate, int limit)
{
	return (coordinate > 0 && coordinate <= limit);
}

void Get_Data()
{
	ifstream input("in.txt");
	input >> N >> M;


	for (int i = 1; i <= N; i++)
	{
		for (int j = 1; j <= M; j++)
		{
			input >> ChessFieldVal[i][j];
			DistanceTo[i][j] = INT_MAX;
		}

	}
	input >> X1 >> Y1 >> X2 >> Y2;

	input.close();
}


void Set_Defaults()
{
	Jump[0] = make_pair(-1, -2);
	Jump[1] = make_pair(1, -2);
	Jump[2] = make_pair(-1, 2);
	Jump[3] = make_pair(1, 2);
	Jump[4] = make_pair(2, 1);
	Jump[5] = make_pair(2, -1);
	Jump[6] = make_pair(-2, 1);
	Jump[7] = make_pair(-2, -1);

	DistanceTo[X1][Y1] = 0;
	set_ToVisit.insert(make_pair(DistanceTo[X1][Y1], make_pair(X1, Y1)));
}


void Print_Answer()
{
	ofstream output("out.txt");

	if (DistanceTo[X2][Y2] == INT_MAX)
		output << ANSWER_NO;
	else
	{
		output << DistanceTo[X2][Y2];
	}
	output.close();
}


int Jump_Distance(int cellValue)
{
	return cellValue == ID_EMPTY ? PRICE_EMPTY : PRICE_BLACK;
}



void Get_All_Jumps_From(pair<int, int> FromPoint, vector< pair<int, int> >& Vec_SavedJumps)
{
	int CurrentJump = 0, NewX, NewY;
	while (CurrentJump < QT_JUMPS)
	{
		NewX = FromPoint.first + Jump[CurrentJump].first;
		NewY = FromPoint.second + Jump[CurrentJump].second;
		if (Suitable(NewX, N) && Suitable(NewY, M) && ChessFieldVal[NewX][NewY] != ID_WHITE)
		{
			Vec_SavedJumps.emplace_back(make_pair(NewX, NewY));
		}

		++CurrentJump;
	}
}


void Start_Process()
{
	while (!set_ToVisit.empty())
	{
		pair<int, int> CurrentPos = set_ToVisit.begin()->second;
		vector< pair<int, int> > vec_PossibleJumps;
		vector< pair<int, int> >::iterator NextPos_it;
		Get_All_Jumps_From(CurrentPos, vec_PossibleJumps);

		set_ToVisit.erase(set_ToVisit.begin());
		for (NextPos_it = vec_PossibleJumps.begin(); NextPos_it != vec_PossibleJumps.end(); NextPos_it++)
		{
			int DistanceToNext = Jump_Distance(ChessFieldVal[NextPos_it->first][NextPos_it->second]);
			int NewDistance = DistanceTo[CurrentPos.first][CurrentPos.second] + DistanceToNext;
			if (NewDistance < DistanceTo[NextPos_it->first][NextPos_it->second])
			{
				set_ToVisit.erase({DistanceTo[NextPos_it->first][NextPos_it->second], *NextPos_it});
				DistanceTo[NextPos_it->first][NextPos_it->second] = NewDistance;
				set_ToVisit.insert({ DistanceTo[NextPos_it->first][NextPos_it->second], *NextPos_it });
			}
		}
	}
}


int main()
{
	Get_Data();
	Set_Defaults();

	Start_Process();

	Print_Answer();

	return 0;
}