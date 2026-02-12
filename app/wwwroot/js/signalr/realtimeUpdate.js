var connection = new signalR.HubConnectionBuilder().withUrl("/HospitalInfoHub").build();


connection.on("ReceiveMessage", function (placeId, waitTime,beds) {
    //var li = document.createElement("li");
    //document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you
    // should be aware of possible script injection concerns.
    //li.textContent = `${user} says ${message}`;

    console.log('Msg received : ' + placeId + ', ' + waitTime + ', ' + beds);
    const cardId = "#hospital_card_" + placeId + "";
    var targetCard = document.querySelector(cardId);

    if (typeof targetCard != "undefined") {
        let estimatedTime = parseInt(waitTime);
        if (estimatedTime >= 60) {
            targetCard.querySelector(cardId + " .estimated-wait-time").innerHTML = Math.floor(estimatedTime / 60) + "h " + (estimatedTime % 60) + "m";
            
        } else {
            targetCard.querySelector(cardId + " .estimated-wait-time").innerHTML = estimatedTime + "m";
        }
        document.querySelector("#hospital_marker_ewt_" + placeId).innerHTML = targetCard.querySelector(".estimated-wait-time").innerHTML;


        targetCard.querySelector(".open-beds").innerHTML = beds + "";
    }
});

connection.start().then(function () {
    // document.getElementById("sendButton").disabled = false;
    console.log("Connection started")
}).catch(function (err) {
    return console.error(err.toString());
});

sendHospitalUpdateInfo = function (placeId, waitTime, beds) {
    if (typeof connection != "undefined") {
        connection.invoke("SendMessage", placeId, waitTime, beds).catch(function (err) {
            return console.error(err.toString());
        });
    }
};

/*
document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

*/