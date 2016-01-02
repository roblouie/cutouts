'use strict';
class Player {
    constructor(x, y){
        // (x, y) = center of object
        // ATTENTION:
        // it represents the player position on the world(room), not the canvas position
        this.x = x;
        this.y = y;

        // move speed in pixels per second
        this.speed = 200;

        // render properties
        this.width = 50;
        this.height = 50;
    }

    update(step, worldWidth, worldHeight){
        // parameter step is the time between frames ( in seconds )

        // check controls and move the player accordingly
        if(Game.controls.left)
            this.x -= this.speed * step;
        if(Game.controls.up)
            this.y -= this.speed * step;
        if(Game.controls.right)
            this.x += this.speed * step;
        if(Game.controls.down)
            this.y += this.speed * step;

        // don't let player leaves the world's boundary
        if(this.x - this.width/2 < 0){
            this.x = this.width/2;
        }
        if(this.y - this.height/2 < 0){
            this.y = this.height/2;
        }
        if(this.x + this.width/2 > worldWidth){
            this.x = worldWidth - this.width/2;
        }
        if(this.y + this.height/2 > worldHeight){
            this.y = worldHeight - this.height/2;
        }
    }

    draw(context, xView, yView){
        // draw a simple rectangle shape as our player model
        context.save();
        context.fillStyle = "black";
        // before draw we need to convert player world's position to canvas position
        context.fillRect((this.x-this.width/2) - xView, (this.y-this.height/2) - yView, this.width, this.height);
        context.restore();
    }
}
