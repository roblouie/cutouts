'use strict';
import {GameEngine} from './game-engine';

var gameEngine = new GameEngine();
gameEngine.setupRequestAnimationFrame();

window.Game = {};

(function(){
    // prepaire our game canvas
    var canvas = document.getElementById("gameCanvas");
    var context = canvas.getContext("2d");

    // game settings: // <-- some game settings were removed because requestAnimationFrame controls the screen automatically
    //var FPS = 30; <--removed
    //var INTERVAL = 1000/FPS; // milliseconds <--removed
    var last = 0; // last frame timestamp
    var now = 0; // current timestamp
    var step = now-last; // time between frames

    // setup an object that represents the room
    var room = {
        width: 5000,
        height: 3000,
        map: new Map(5000, 3000)
    };

    // generate a large image texture for the room
    room.map.generate();

    // setup player
    var player = new Player(50, 50);

    // setup the magic camera !!!
    var camera = new Camera(0, 0, canvas.width, canvas.height, room.width, room.height);
    camera.follow(player, canvas.width/2, canvas.height/2);

    // Game update function
    var update = function(step){
        player.update(step, room.width, room.height); // <-- edited
        camera.update();
    }

    // Game draw function
    var draw = function(){
        // clear the entire canvas
        context.clearRect(0, 0, canvas.width, canvas.height);

        // redraw all objects
        room.map.draw(context, camera.xView, camera.yView);
        player.draw(context, camera.xView, camera.yView);
    }

    var runningId = -1;

    // Game Loop
    var gameLoop = function(timestamp){ // <-- edited; timestamp comes from requestAnimationFrame. See polyfill to get this insight.
        now = timestamp; // <-- current timestamp (in milliseconds)
        step = (now-last)/1000; // <-- time between frames (in seconds)
        last = now; // <-- store the current timestamp for further evaluation in next frame/step

        update(step);
        draw();
        runningId = requestAnimationFrame(gameLoop); // <-- added
    }

    // ---configure play/pause capabilities:

    Game.play = function(){
        if(runningId == -1){
            runningId = requestAnimationFrame(gameLoop); // <-- changed
            console.log("play");
        }
    }

    Game.togglePause = function(){
        if(runningId == -1){
            Game.play();
        }
        else
        {
            cancelAnimationFrame(runningId);// <-- changed
            runningId = -1;
            console.log("paused");
        }
    }

    // ---

})();

Game.controls = {
    left: false,
    up: false,
    right: false,
    down: false
};

window.addEventListener("keydown", function(e){
    switch(e.keyCode)
    {
        case 37: // left arrow
            Game.controls.left = true;
            break;
        case 38: // up arrow
            Game.controls.up = true;
            break;
        case 39: // right arrow
            Game.controls.right = true;
            break;
        case 40: // down arrow
            Game.controls.down = true;
            break;
    }
}, false);

window.addEventListener("keyup", function(e){
    switch(e.keyCode)
    {
        case 37: // left arrow
            Game.controls.left = false;
            break;
        case 38: // up arrow
            Game.controls.up = false;
            break;
        case 39: // right arrow
            Game.controls.right = false;
            break;
        case 40: // down arrow
            Game.controls.down = false;
            break;
        case 80: // key P pauses the game
            Game.togglePause();
            break;
    }
}, false);

window.onload = function(){
    Game.play();
}

