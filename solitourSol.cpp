#include <fstream>
#include <cmath>
#include <inttypes.h>

#define		FILE_NAME_INPUT "input.txt"
#define		FILE_NAME_OUTPUT "output.txt"

using namespace std;
//GLOBALS--------------------------------------
///index - CardValue; CardPosition[index] - position by default
int16_t CardPosition[502], QtCards;
int DpMatrix[502][502];
//---------------------------------------------



void Get_Data()
{
	int16_t cardvalue;
	ifstream input(FILE_NAME_INPUT);

	input >> QtCards;

	for (int i = 1; i <= QtCards; i++)
	{
		input >> cardvalue;
		CardPosition[cardvalue] = i;
	}

	input.close();
}


void Set_Dp_Base()
{
    for(int16_t i = 1; i <= QtCards; i++)
    {
        DpMatrix[i][i] = 0;
    }

}


int Min_Between(const int _start, const int _finish)
{
	int MinValue = INT_MAX, StepsAsDivided;
	for (int divider = _start; divider < _finish; divider++)
	{
		StepsAsDivided = abs(CardPosition[divider] - CardPosition[_finish]) + 
							DpMatrix[_start][divider] + DpMatrix[divider + 1][_finish];
		if (StepsAsDivided < MinValue)
		{
			MinValue = StepsAsDivided;
		}
	}

	return MinValue;
}


void Dp_Calculation()
{
    if (QtCards > 1)
    {
        int16_t CurrentColumn = 2, i, j, Limit;
        while(CurrentColumn <= QtCards)
        {
            i = 1; j = CurrentColumn;
            Limit = QtCards - CurrentColumn + 1;
            while(i <= Limit)
            {
                DpMatrix[i][j] = Min_Between(i, j);
                ++i; ++j;
            }

			++CurrentColumn;
		}

    }
}


void Print_Answer()
{
    ofstream output(FILE_NAME_OUTPUT);
    
	output << DpMatrix[1][QtCards];

    output.close();
}

int main()
{
	Get_Data();

    Set_Dp_Base();

    Dp_Calculation();

    Print_Answer();

	return 0;
}