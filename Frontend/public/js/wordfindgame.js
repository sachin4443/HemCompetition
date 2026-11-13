// Arvind Pandey....Created...
var Global_CourseID;
var GetPractID;

(function (document, $, wordfind) {

    'use strict';

    var WordFindGame = function () {

        // ---------------------------------------------------------
        // List of words for this game
        // ---------------------------------------------------------
        var wordList;


        /**
         * Draws the puzzle by inserting rows of buttons into el.
         */
        var drawPuzzle = function (el, puzzle) {

            var output = '';

            // for each row in the puzzle
            for (var i = 0, height = puzzle.length; i < height; i++) {

                var row = puzzle[i];

                output += '<div>';

                // for each element in that row
                for (var j = 0, width = row.length; j < width; j++) {

                    output += '<button class="puzzleSquare" x="' + j + '" y="' + i + '">';

                    output += row[j] || '&nbsp;';

                    output += '</button>';
                }

                // close row
                output += '</div>';
            }

            $(el).html(output);
        };


        /**
         * Draws the words by inserting an unordered list into el.
         */
        var drawWords = function (el, wordObj) {

            var output = '<ul>';

            for (var word in wordObj) {

                var definition = wordObj[word];

                output += '<li class="word ' + "wf_" + word + '">' + definition + '</li>';
            }

            output += '</ul>';

            $(el).html(output);
        };


        // ---------------------------------------------------------
        // GAME STATE
        // ---------------------------------------------------------
        var startSquare,
            selectedSquares = [],
            curOrientation,
            curWord = '';


        /**
         * Event that handles mouse down on a new square.
         */
        var startTurn = function () {

            $(this).addClass('selected');

            startSquare = this;

            selectedSquares.push(this);

            curWord = $(this).text();

            event.preventDefault();
        };


        /**
         * Event that handles mouse over on a new square.
         */
        var select = function (target) {

            // If target is invalid
            if (!target) {
                return;
            }

            // If the user hasn't started a word yet
            if (!startSquare) {
                return;
            }

            // If same square
            var lastSquare =
                selectedSquares[selectedSquares.length - 1];

            if (lastSquare == target) {
                return;
            }


            // Check if user backed up
            var backTo;

            for (
                var i = 0,
                    len = selectedSquares.length;
                i < len;
                i++
            ) {

                if (selectedSquares[i] == target) {

                    backTo = i + 1;

                    break;
                }
            }


            while (backTo < selectedSquares.length) {

                $(selectedSquares[selectedSquares.length - 1])
                    .removeClass('selected');

                selectedSquares.splice(backTo, 1);

                curWord =
                    curWord.substr(
                        0,
                        curWord.length - 1
                    );
            }


            // Check orientation
            var newOrientation = calcOrientation(
                $(startSquare).attr('x') - 0,
                $(startSquare).attr('y') - 0,
                $(target).attr('x') - 0,
                $(target).attr('y') - 0
            );


            if (newOrientation) {

                selectedSquares = [startSquare];

                curWord = $(startSquare).text();

                if (lastSquare !== startSquare) {

                    $(lastSquare)
                        .removeClass('selected');

                    lastSquare = startSquare;
                }

                curOrientation = newOrientation;
            }


            var orientation = calcOrientation(
                $(lastSquare).attr('x') - 0,
                $(lastSquare).attr('y') - 0,
                $(target).attr('x') - 0,
                $(target).attr('y') - 0
            );


            if (!orientation) {
                return;
            }


            if (
                !curOrientation ||
                curOrientation === orientation
            ) {

                curOrientation = orientation;

                playTurn(target);
            }
        };


        /**
         * Touch move
         */
        var touchMove = function (e) {

            if (
                !e.originalEvent ||
                !e.originalEvent.touches ||
                !e.originalEvent.touches.length
            ) {
                return;
            }

            var xPos =
                e.originalEvent.touches[0].pageX;

            var yPos =
                e.originalEvent.touches[0].pageY;

            var targetElement =
                document.elementFromPoint(
                    xPos,
                    yPos
                );

            select(targetElement);
        };


        /**
         * Mouse move
         */
        var mouseMove = function () {

            select(this);
        };


        /**
         * Updates the game state when previous selection was valid.
         */
        var playTurn = function (square) {

            for (
                var i = 0,
                    len = wordList.length;
                i < len;
                i++
            ) {

                if (
                    wordList[i].indexOf(
                        curWord + $(square).text()
                    ) === 0
                ) {

                    $(square).addClass('selected');

                    selectedSquares.push(square);

                    curWord += $(square).text();

                    break;
                }
            }
        };


        /**
         * Event that handles mouse up.
         */
        var endTurn = function () {

            // Check valid word
            for (
                var i = 0,
                    len = wordList.length;
                i < len;
                i++
            ) {

                if (wordList[i] === curWord) {

                    // Mark selected squares as found
                    $('.selected').addClass('found');

                    // Remove word from remaining list
                    wordList.splice(i, 1);

                    // Mark word chip as found
                    $('.' + "wf_" + curWord)
                        .addClass('wordFound')
                        .html(curWord);

                    // Existing callback
                    completeWordHandler(curWord);

                    break;
                }
            }


            // -----------------------------------------------------
            // PUZZLE COMPLETED
            // -----------------------------------------------------
            if (wordList.length === 0) {

                CheckCompletionGame(wordList.length);

                $('.puzzleSquare')
                    .addClass('complete');
            }


            // Reset turn
            $('.selected').removeClass('selected');

            startSquare = null;

            selectedSquares = [];

            curWord = '';

            curOrientation = null;

            event.preventDefault();
        };


        /**
         * Add complete word callback.
         */
        var completeWordHandler = function (word) {

        };


        // ---------------------------------------------------------
        // SAVE COMPLETED PUZZLE
        // ---------------------------------------------------------
        var CheckCompletionGame = function (lengh) {

            var ST = d;

            console.log("Puzzle start time:", d);

            var IN_H =
                new Date(d).getHours();

            var IN_M =
                new Date(d).getMinutes();

            var IN_S =
                new Date(d).getSeconds();


            const o = new Date();

            var ET = o;


            var OUT_H =
                new Date(o).getHours();

            var OUT_M =
                new Date(o).getMinutes();

            var OUT_S =
                new Date(o).getSeconds();


            var GetF_H =
                parseInt(OUT_H) -
                parseInt(IN_H);

            var GetF_M =
                parseInt(OUT_M) -
                parseInt(IN_M);

            var GetF_S =
                parseInt(OUT_S) -
                parseInt(IN_S);


            var HrsDiff =
                Math.floor(
                    GetF_H * 60 * 60 +
                    GetF_M * 60 +
                    GetF_S
                );

            var SecDiff =
                Math.floor(
                    GetF_M * 60 +
                    GetF_S
                );

            var DIFF =
                GetF_H +
                " Hrs : " +
                GetF_M +
                " Mins : " +
                GetF_S +
                " Secs";


            // -----------------------------------------------------
            // ONLY SAVE WHEN ALL WORDS ARE COMPLETED
            // -----------------------------------------------------
            if (lengh === 0) {

                var diffSeconds =
                    Math.floor(
                        (ET - d) / 1000
                    );


                console.log(
                    "Saving completed puzzle..."
                );


                $.ajax({

                    type: 'POST',

                    contentType:
                        'application/json',

                    url:
                        '/api/competition/puzzle-save',

                    async: true,

                    headers: {

                        'Authorization':
                            'Bearer ' +
                            localStorage.getItem(
                                'jwt_token'
                            )
                    },

                    data: JSON.stringify({

                        startTime:
                            d.toString(),

                        endTime:
                            ET.toString(),

                        timeDiff:
                            diffSeconds.toString(),

                        userAnswer:
                            "1",

                        remark:
                            null
                    }),


                    // -------------------------------------------------
                    // SUCCESS
                    // -------------------------------------------------
                    success: function (response) {

                        console.log(
                            "Record saved successfully:",
                            response
                        );


                        /*
                         * IMPORTANT:
                         *
                         * NO Swal.fire()
                         * NO window.location.href
                         * NO page reload
                         *
                         * React PuzzlePage listens to this event
                         * and automatically loads next puzzle.
                         */

                        try {

                            window.dispatchEvent(
                                new CustomEvent(
                                    'wordfind:puzzle-saved',
                                    {
                                        detail: {
                                            response:
                                                response,

                                            startTime:
                                                d,

                                            endTime:
                                                ET,

                                            timeDiff:
                                                diffSeconds
                                        }
                                    }
                                )
                            );

                        } catch (eventError) {

                            console.error(
                                "Unable to dispatch puzzle saved event:",
                                eventError
                            );
                        }
                    },


                    // -------------------------------------------------
                    // ERROR
                    // -------------------------------------------------
                    error: function (response) {

                        console.error(
                            "Error saving puzzle:",
                            response
                        );


                        /*
                         * Notify React that save failed.
                         *
                         * React will show the error
                         * on the same screen.
                         */

                        try {

                            window.dispatchEvent(
                                new CustomEvent(
                                    'wordfind:puzzle-save-error',
                                    {
                                        detail: {
                                            response:
                                                response
                                        }
                                    }
                                )
                            );

                        } catch (eventError) {

                            console.error(
                                "Unable to dispatch puzzle save error event:",
                                eventError
                            );
                        }
                    }

                });
            }
        };


        /**
         * Given two points, ensure that they are adjacent
         * and determine orientation.
         */
        var calcOrientation = function (
            x1,
            y1,
            x2,
            y2
        ) {

            for (
                var orientation in wordfind.orientations
            ) {

                var nextFn =
                    wordfind.orientations[
                        orientation
                    ];

                var nextPos =
                    nextFn(
                        x1,
                        y1,
                        1
                    );


                if (
                    nextPos.x === x2 &&
                    nextPos.y === y2
                ) {

                    return orientation;
                }
            }


            return null;
        };


        /**
         * Return sorted wordList.
         */
        var getWordList = function (words) {

            if ($.isArray(words)) {

                return words
                    .slice(0)
                    .sort();

            } else if ($.isPlainObject(words)) {

                var keys = [];

                for (
                    var key in words
                ) {

                    keys.push(key);
                }

                return keys
                    .slice(0)
                    .sort();
            }
        };


        /**
         * Return object of word:definition.
         */
        var getWordObj = function (words) {

            if ($.isArray(words)) {

                var wordObj = {};

                for (
                    var i in words
                ) {

                    wordObj[words[i]] =
                        words[i];
                }

                return wordObj;

            } else if ($.isPlainObject(words)) {

                return words;
            }
        };


        // ---------------------------------------------------------
        // PUBLIC API
        // ---------------------------------------------------------
        return {

            /**
             * Creates a new word find game.
             */
            create: function (
                words,
                puzzleEl,
                wordsEl,
                options,
                options1
            ) {

                Global_CourseID =
                    options;

                GetPractID =
                    options1;


                var wordObj =
                    getWordObj(words);


                wordList =
                    getWordList(words);


                window.test =
                    wordList;

                document.test =
                    wordList;


                var puzzle =
                    wordfind.newPuzzle(
                        wordList,
                        options
                    );


                // Draw puzzle
                drawPuzzle(
                    puzzleEl,
                    puzzle
                );


                // Draw words
                drawWords(
                    wordsEl,
                    wordObj
                );


                // Attach events
                if (
                    window.navigator.msPointerEnabled
                ) {

                    $('.puzzleSquare')
                        .on(
                            'pointerdown',
                            startTurn
                        );

                    $('.puzzleSquare')
                        .on(
                            'pointerover',
                            select
                        );

                    $('.puzzleSquare')
                        .on(
                            'pointerup',
                            endTurn
                        );

                    $('.puzzleSquare')
                        .mousedown(
                            startTurn
                        );

                    $('.puzzleSquare')
                        .mouseenter(
                            mouseMove
                        );

                    $('.puzzleSquare')
                        .mouseup(
                            endTurn
                        );

                } else {

                    $('.puzzleSquare')
                        .mousedown(
                            startTurn
                        );

                    $('.puzzleSquare')
                        .mouseenter(
                            mouseMove
                        );

                    $('.puzzleSquare')
                        .mouseup(
                            endTurn
                        );

                    $('.puzzleSquare')
                        .on(
                            "touchstart",
                            startTurn
                        );

                    $('.puzzleSquare')
                        .on(
                            "touchmove",
                            touchMove
                        );

                    $('.puzzleSquare')
                        .on(
                            "touchend",
                            endTurn
                        );
                }


                return puzzle;
            },


            /**
             * Solves existing puzzle.
             */
            solve: function (
                puzzle,
                words
            ) {

                var solution =
                    wordfind.solve(
                        puzzle,
                        words
                    ).found;


                for (
                    var i = 0,
                        len = solution.length;
                    i < len;
                    i++
                ) {

                    var word =
                        solution[i].word;

                    var orientation =
                        solution[i].orientation;

                    var x =
                        solution[i].x;

                    var y =
                        solution[i].y;

                    var next =
                        wordfind.orientations[
                            orientation
                        ];


                    if (
                        !$(
                            '.' + word
                        ).hasClass(
                            'wordFound'
                        )
                    ) {

                        for (
                            var j = 0,
                                size = word.length;
                            j < size;
                            j++
                        ) {

                            var nextPos =
                                next(
                                    x,
                                    y,
                                    j
                                );

                            $(
                                '[x="' +
                                nextPos.x +
                                '"][y="' +
                                nextPos.y +
                                '"]'
                            ).addClass(
                                'solved'
                            );
                        }


                        $('.' + word)
                            .addClass(
                                'wordFound'
                            );
                    }
                }
            },


            /**
             * Add complete word callback.
             */
            onClompleteWord: function (call) {

                if (
                    typeof call == "function"
                ) {

                    completeWordHandler =
                        call;

                } else {

                    console.error(
                        "onClompleteWord expected a function!"
                    );
                }
            }
        };
    };


    /**
     * Allow game to be used within browser.
     */
    window.wordfindgame =
        WordFindGame();

}(document, jQuery, wordfind));