#include <fstream>
#include <iostream>
#include <cmath>

#define		QT_TO_MULTI 2
using namespace std;
int QtMatrix;
pair<int, int> MSize[1001];
int Multiplications[1001][1001];



using namespace std;


int Qt_Operations(int FirstIndex, int SecondIndex)
{
	return MSize[FirstIndex].first * MSize[FirstIndex].second * MSize[SecondIndex].second;
}


//govno code
void Create_Base()
{
	int i = 0, j = 0;
	while (i < QtMatrix)
	{
		Multiplications[i][j] = 0;
		++i;
		++j;
	}

	i = 0; j = 1;
	while (i < QtMatrix - 1)
	{
		Multiplications[i][j] = Qt_Operations(i, j);
		++i;
		++j;
	}
}


int Multiply_Matrix(int f, int s)
{
	int Qt = 0;
	if (s - f > 1)
	{
		int	CurrentOperations, MinOperations = Multiplications[f + 1][s] + Qt_Operations(f, s);
	
		for (int k = f + 1; k < s; k++)
		{
			CurrentOperations = Multiplications[f][k] + Multiplications[k + 1][s] + MSize[f].first * MSize[k].second * MSize[s].second;
			if (CurrentOperations < MinOperations)
				MinOperations = CurrentOperations;
		}

		Qt = MinOperations;
	}
	else
	{
		if (s == f)
			Qt = 0;
		else
			Qt = Qt_Operations(f, s);
	}

	return Qt;
}


void Calc_Operations()
{
	if (QtMatrix > QT_TO_MULTI)
	{
		int i = 0, CurStartColumn= QT_TO_MULTI, j;
		while (CurStartColumn <= QtMatrix - 1)
		{
			i = 0; j = CurStartColumn;
			while (i < QtMatrix - CurStartColumn)
			{
				Multiplications[i][j] = Multiply_Matrix(i, j);
				++i;
				++j;
			}
			
			++CurStartColumn;
		}
	}
}


int main()
{
	ifstream input("input.txt");
	int N, M;

	input >> QtMatrix;

	for (int i = 0; i < QtMatrix; i++)
	{
		input >> N >> M;
		MSize[i] = make_pair(N, M);
	}
	input.close();

	Create_Base();
	Calc_Operations();

	ofstream output("output.txt");

	output << Multiplications[0][QtMatrix - 1];

	output.close();

	return 0;
}