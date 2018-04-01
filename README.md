# Char Control Real Time C#

## About

CharControlRealTime is a controlka which easily enables the presentation of data in real time. The user may
- zoom in any part of the plot
- scale the graph
- set the chart margin
- change the size of the FIFO buffor (the amount of data presented)
- read each point of the graph by hovering over the mouse
- enable / disable drawing the chart in real time

## Creating your first Char

Char is the name of the control used in XAML

Adding new points to the chart is done by using the AddPoint method
 ```csharp
 Char.AddPoint(x, y);
 ```
 
 Adding a legend to the chart is done by calling the AddLegend method. The addition of a legend is optional
 ```csharp
 Char.AddLegend("Your Legend");
 ```

<img src="https://media.giphy.com/media/7YCARhnT7AwIIikDPj/giphy.gif" width="100%" height="120%" />

## License

This project uses the [GNU General Public License v3.0](https://www.gnu.org/licenses/gpl-3.0.en.html).
