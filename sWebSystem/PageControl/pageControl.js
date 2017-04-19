var swCon = new sWebSystem.sWebConverter();
var stHan = new sWebSystem.sSceneHandler();
var edgeColor = new THREE.Color(0.7, 0.7, 0.7)

var currentColorMode = sWebSystem.eColorMode.Stress_Combined_Absolute;


$(document).ready(function () {

    stHan.InitiateASKSGHScene(false);

    stHan.AddBaseGrid("grid", 10, 10);
    stHan.AddBaseAxisLines("axis", 10);

    stHan.ImportLocalJsonsSystem("testSystem", currentColorMode, 0.0, edgeColor);

    //var t = stHan.LoadLocalJsonSystem("testSystem");

    stHan.updateFunctions.push(UpdateColorTest);

    stHan.AninmateScene();
    
});



var count = 0;
var colth = 50000;
function UpdateColorTest() {
    //if (count == 30) {
        count = 0;
        // Check All Data Type
        //Stress Moment Need Fix
        // Figure Out How to Switch Texture Map...
        stHan.UpdatesSystemColor(sWebSystem.eColorMode.Stress_Combined_Absolute, colth);
        colth *= 1.01;
    //}
    count++;
    console.log(count);
}
