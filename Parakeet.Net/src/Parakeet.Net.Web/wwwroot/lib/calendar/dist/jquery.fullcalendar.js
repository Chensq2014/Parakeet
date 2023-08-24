$(document).ready(function () {
	var drag = function () {
		$('.calendar-event').each(function () {
			// store data so the calendar knows to render an event upon drop
			$(this).data('event', {
				title: $.trim($(this).text()), // use the element's text as the event title
				stick: true // maintain when user navigates (see docs on the renderEvent method)
			});

			// make the event draggable using jQuery UI
			$(this).draggable({
				zIndex: 1111999,
				revert: true, // will cause the event to go back to its
				revertDuration: 0 //  original position after the drag
			});
		});
	};

	var removeEvent = function () {
		$('.remove-calendar-event').click(function () {
			$(this).closest('.calendar-event').fadeOut();
			return false;
		});
	};

	$(".add-event").keypress(function (e) {
		if ((e.which == 13) && (!$(this).val().length == 0)) {
			$('<div class="calendar-event">' + $(this).val() + '<a href="javascript:void(0);" class="remove-calendar-event"><i class="ti-close"></i></a></div>').insertBefore(".add-event");
			$(this).val('');
		} else if (e.which == 13) {
			alert('Please enter event name');
		}
		drag();
		removeEvent();
	});

	drag();
	removeEvent();

	var date = new Date();
	var day = date.getDate();
	var month = date.getMonth();
	var year = date.getFullYear();

	$('#calendar').fullCalendar({
		header: {
			left: 'prev,next today',
			center: 'title',
			right: 'month,agendaWeek,agendaDay'
		},
		slotDuration: '00:15:00', /* If we want to split day time each 15minutes */
        minTime: '08:00:00',
        maxTime: '19:00:00',  
		editable: true,
		droppable: true, // this allows things to be dropped onto the calendar
		eventLimit: true, // allow "more" link when too many events
		events: []
	});
});
