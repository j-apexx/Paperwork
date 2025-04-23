/* Thanks to Glen M Taylor and Chris Coyle for developing and publishing this calculator
 * https://code.tutsplus.com/tutorials/creating-an-incredible-jquery-calculator--pre-8813
 * https://www.glenmtaylor.com/jcalc.html
 * Modified slightly to make css encapsulated so that it does not affect the entire page */

function resetCalculator(curValue) {
	$("#display").val(curValue);
	$(".function-button").removeClass("pendingFunction");
	$("#display").data("isPendingFunction", false);
	$("#display").data("thePendingFunction", "");
	$("#display").data("valueOneLocked", false);
	$("#display").data("valueTwoLocked", false);
	$("#display").data("valueOne", curValue);
	$("#display").data("valueTwo", 0);
	$("#display").data("fromPrevious", false);
}

function debugStates() {
	console.log("------------")
	console.log("isPendingFunction: " + $("#display").data("isPendingFunction"));
	console.log("thePendingFunction: " + $("#display").data("thePendingFunction"));
	console.log("valueOneLocked: " + $("#display").data("valueOneLocked"));
	console.log("valueTwoLocked: " + $("#display").data("valueTwoLocked"));
	console.log("valueOne: " + $("#display").data("valueOne"));
	console.log("valueTwo: " + $("#display").data("valueTwo"));
	console.log("fromPrevious: " + $("#display").data("fromPrevious"));
};

$(function(){
	
	resetCalculator("0");
	
	$(".num-button").click(function(){
		
		if ($("#display").data("fromPrevious") == true) {
		
			resetCalculator($(this).text());
			
		} else if (($("#display").data("isPendingFunction") == true) && ($("#display").data("valueOneLocked") == false)) {
			
			$("#display").data("valueOne", $("#display").val());
			$("#display").data("valueOneLocked", true);
			
			$("#display").val($(this).text());
			$("#display").data("valueTwo", $("#display").val());
			$("#display").data("valueTwoLocked", true);
		
		// Clicking a number AGAIN, after first number locked and already value for second number	
		} else if (($("#display").data("isPendingFunction") == true) && ($("#display").data("valueOneLocked") == true)) {

			var curValue = $("#display").val();
			var toAdd = $(this).text();
		
			var newValue = curValue + toAdd;
		
			$("#display").val(newValue);
			
			$("#display").data("valueTwo", $("#display").val());
			$("#display").data("valueTwoLocked", true);
		
		// Clicking on a number fresh	
		} else {
		
			var curValue = $("#display").val();
			if (curValue == "0") {
				curValue = "";
			}
		
			var toAdd = $(this).text();
		
			var newValue = curValue + toAdd;
		
			$("#display").val(newValue);
		
		}
			
	});
	// Start Clear Button
	$(".clear-button").click(function(){
		resetCalculator("0");
	});
	// End Clear Button
	
	//Start Change Sign of Number
	$(".plusminus-button").click(function(){
	
		if (($("#display").data("valueOneLocked") == false) && ($("#display").data("valueTwoLocked") == false)) {
			var changesign = $("#display").val();
			changesign = changesign * -1;
			$("#display").val(changesign);
		}
		else if (($("#display").data("valueOneLocked") == true)) {
			var changesign = $("#display").data("valueTwo");
			changesign = changesign * -1;
			$("#display").data("valueTwo", changesign);
			$("#display").val(changesign);
		}
		else if (($("#display").data("valueTwoLocked") == true)) {
			var changesign = $("#display").data("valueOne");
			changesign = changesign * -1;
			$("#display").data("valueOne", changesign);
			$("#display").val(changesign);
		}
	
	});
	
	// Percent operations
	$(".percent-button").click(function(){
		
		if (($("#display").data("valueOneLocked") == false) && ($("#display").data("valueTwoLocked") == false)) {
			var percentNum = $("#display").val();
			percentNum = percentNum / 100;
			$("#display").val(percentNum);
		}
		else if (($("#display").data("valueTwoLocked") == true) && ($("#display").data("thePendingFunction") == "–") || ($("#display").data("thePendingFunction") == "+"))  {
			var percentNum = $("#display").data("valueTwo");
			var newNum = $("#display").data("valueOne");
			percentNum = percentNum / 100;
			newNum = newNum * percentNum;
			$("#display").val(newNum);
			$("#display").data("valueTwo", newNum);
		}
		else if (($("#display").data("valueTwoLocked") == true) && ($("#display").data("thePendingFunction") == "x") || ($("#display").data("thePendingFunction") == String.fromCharCode(247))) {
			var percentNum = $("#display").data("valueTwo");
			percentNum = percentNum / 100;
			$("#display").val(percentNum);
			$("#display").data("valueTwo", percentNum);
		}
	
	
	});
	
	$(".function-button").click(function(){

		if ($("#display").data("fromPrevious") == true) {
			resetCalculator($("#display").val());
			$("#display").data("valueOneLocked", false);
			$("#display").data("fromPrevious", false)
		}
		
		// Let it be known that a function has been selected
		var pendingFunction = $(this).text();
		$("#display").data("isPendingFunction", true);
		$("#display").data("thePendingFunction", pendingFunction);
		
		// Visually represent the current function
		$(".function-button").removeClass("pendingFunction");
		$(this).addClass("pendingFunction");
	});
	
	
	$(".equals-button").click(function(){
	
		if (($("#display").data("valueOneLocked") == true) && ($("#display").data("valueTwoLocked") == true)) {
			
			if ($("#display").data("thePendingFunction") == "+") {
				var finalValue = parseFloat($("#display").data("valueOne")) + parseFloat($("#display").data("valueTwo"));
			} else if ($("#display").data("thePendingFunction") == "–") {
				var finalValue = parseFloat($("#display").data("valueOne")) - parseFloat($("#display").data("valueTwo"));
			} else if ($("#display").data("thePendingFunction") == "x") {
				var finalValue = parseFloat($("#display").data("valueOne")) * parseFloat($("#display").data("valueTwo"));
			} else if ($("#display").data("thePendingFunction") == String.fromCharCode(247)) {
				var finalValue = parseFloat($("#display").data("valueOne")) / parseFloat($("#display").data("valueTwo"));
			}
			
	
			
			if (finalValue % 1 != 0) {
				var decimalPlaces = finalValue.toString().split(".")[1].length;
					if (decimalPlaces > 6) {
						finalValue = finalValue.toPrecision(6);
						$("#display").val(finalValue); 
					}
					else {
						finalValue = finalValue.toPrecision();
						$("#display").val(finalValue);
					}
			}
			else {
				$("#display").val(finalValue);
			}
			
		
			resetCalculator(finalValue);
			$("#display").data("fromPrevious", true);
						
		} else {
			// both numbers are not locked, do nothing.
		}
		
	});
	
	$("#calculator").draggable();

	
	$("#opener, #closer").click(function(){
		$("#opener").toggle();
		$("#calculator").toggle();
	});
	
	//Additional Mouse events
	$("#opener, #closer, .numpad").mouseenter(function() {
		$(this).css('cursor','pointer');
	});
	
	$("#closer").mouseenter(function() {
		$(this).css({
			'background': 'white',
			'color': 'black'
		
		})
	});
	
	$("#closer").mouseleave(function() {
		$(this).css({
			'background': 'black',
			'color': 'white'
		
		})
	});
	
});