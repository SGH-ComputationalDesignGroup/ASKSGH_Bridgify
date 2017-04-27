
var stHan = new sWebSystem.sSceneHandler();

$(document).ready(function () {
    stHan.ASKSGHProjectName = "sWebSystem";
    stHan.ASKSGHType = sWebSystem.eASKSGHType.ASKSGH_Gridify;
    stHan.ProxyServerOn();

    stHan.InitialASKSGH();

    stHan.deformFactor = 10;
    //stHan.GetCameraShadowHelper();
    //stHan.AddBaseGrid("grid", 10, 50);

    //stHan.ImportLocalJsonsSystem("testSystem", edgeColor);
    //stHan.ImportLocalJsonsMesh("Test1");

    //stHan.updateFunctions.push(DynamicTest);
    stHan.AninmateScene();
});


var count = 0;
var colth = 50000;
function DynamicTest() {

 //  stHan.UpdatesSystemColor(sWebSystem.eColorMode.Stress_Combined_Absolute, colth);
 //  colth *= 1.01;
 // 
    //
    if(stHan.currentSystem != undefined){
        if (count > 300) {
            stHan.AnimateDeformSystem(true);
        }
        if (count > 600) {
            stHan.AnimateDeformSystem(false);
            count = 0;
        }
        count++;
    }
   
    //console.log(stHan.isTouchDevice);

   // if (stHan.objClicked != undefined) {
   //     console.log(stHan.objClicked.userData.name);
   // }
}

function DeferredTest() {
    var deferred = new $.Deferred();
    setTimeout(function () {

        deferred.resolve();
    }, 1000);
    return deferred.promise();
}
