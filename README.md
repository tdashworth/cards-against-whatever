# Cards Against Whatever

This is a CAH game implementation using Blazor WASM and SignalR communication. 

To play the game, visit https://cax.tdashworth.uk/ or http://card-against-whatever.azurewebsites.net/ then you must supply a card pack file which will generate a game code for you to share. 

> Please note there may be a delay loading the game after a period of inactivity since it's not always running.

## The Card Pack file

I don't want to be liable for sharing the card packs I buy to use and therefore the game was developed for you to supply your own. This is beneficial since you can make your own up or buy a set and manually convert them to digitial like I did. 

This is a CSV file with three columns. 

- `Text` for the card body.
- `Type` is either `Question` or `Answer`
- `Pick` is the number of answer cards to answer this card (required for questions only)

Please see this [file](https://github.com/tdashworth/cards-against-whatever/blob/master/SampleCardDeck.csv) as an example. 

## Issues and Problems 

This is not my full time effort but I am making this open for others to contribute. Please create issues for any problems you discover with as much detail as you can. I will try to resolve these as quickly as I can but I can't provide a reasonable timeframe. 🙁
