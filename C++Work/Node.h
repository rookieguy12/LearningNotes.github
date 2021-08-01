#include <iostream>
#include <vector>
#include <map>
#include <algorithm>
using namespace std;
struct Position
{
    float x;
    float y;
    Position():x(0),y(0){}
};
class Node
{
    private:
        bool isVisited;
        struct Position position;
    public:
        Node();
        vector<Node> connectedNodes;
        float GetDistance(Node other);
};