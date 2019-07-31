const readline = require('readline');

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

const commands = ['left', 'right', 'up', 'down'];
const gameStartOptions = {}
let currentPosition = {}

const fillGameStartOptions = (obj) => {
    gameStartOptions.height = obj.params.y_cells_count;
    gameStartOptions.width = obj.params.x_cells_count;
    gameStartOptions.speed = obj.params.speed;
    gameStartOptions.xell_size = obj.params.width;
};

const fillCurrentPosition = (stateObj) => {
    currentPosition = stateObj.params.players['i'].position;
    currentPosition.direction =  commands.indexOf(stateObj.params.players['i'].direction);
};

const calculateNewCurrentPosition = (direction) => {
    let tmpDirection = direction;
    const newPosition =  {... currentPosition}
    if (direction == null || Math.abs(direction % 2 - 1 ) === currentPosition.direction % 2 &&
        (
            (direction < 2 && currentPosition.direction < 2) || (direction >= 2 && currentPosition.direction >= 2)
        )
    ) {
        tmpDirection = currentPosition.direction
    }
    if (tmpDirection < 2) {
        newPosition.x += (tmpDirection * 2 - 1)*gameStartOptions.speed
    } else {
        newPosition.y -= ((tmpDirection - 2) * 2 - 1)*gameStartOptions.speed
    }
    return newPosition;
};

const checkIsSuicideMove = (direction, stateObj) => {
    let newState = calculateNewCurrentPosition(direction);
    if (newState.x < 0 || newState.x >= gameStartOptions.width ){
        return true;
    }
    if (newState.y < 0 || newState.y >= gameStartOptions.height ){
        return true;
    }

    let tailBlock = stateObj.params.players['i'].lines.find(block => block.x === newState.x && block.y === newState.y);
    return !!tailBlock;
};

const findNewDirection =  (stateObj) => {
    let isForbiddenDirection = true;
    let newDirection = 0;
    while (isForbiddenDirection) {
        newDirection = Math.floor(Math.random() * commands.length);
        isForbiddenDirection = checkIsSuicideMove(newDirection, stateObj);
    }
    return newDirection;
};

let isJustStarted = false;

let handler = (state) => {
    const stateObj = JSON.parse(state);
    let direction = 1;

    if (stateObj.type === 'start_game'){
        fillGameStartOptions(stateObj);
        isJustStarted = true;
    }

    if (stateObj.type === 'tick' && isJustStarted){
        fillCurrentPosition(stateObj)
        isJustStarted = false;
    } else if (stateObj.type === 'tick') {
        direction = findNewDirection(stateObj)
        currentPosition = calculateNewCurrentPosition(direction)
    }

    let command = commands[direction];
    console.log(JSON.stringify({command, debug: command}));
    rl.question('', handler);
};

rl.question('', handler);
