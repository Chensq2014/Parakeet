!function ($) {
	"use strict";

	var CalendarApp = function () {
		this.$body = $("body")
			this.$calendar = $('#calendar'),
		this.$event = ('#calendar-events div.calendar-events'),
		this.$categoryForm = $('#add-new-event form'),
		this.$extEvents = $('#calendar-events'),
		this.$modal = $('#my-event'),
		this.$saveCategoryBtn = $('.save-category'),
		this.$calendarObj = null
	};

	/* on drop */
	CalendarApp.prototype.onDrop = function (eventObj, date) {
		var $this = this;
		// retrieve the dropped element's stored Event Object
		var originalEventObject = eventObj.data('eventObject');
		var $categoryClass = eventObj.attr('data-class');
		// we need to copy it, so that multiple events don't have a reference to the same object
		var copiedEventObject = $.extend({}, originalEventObject);
		// assign it the date that was reported
		copiedEventObject.start = date;
		if ($categoryClass)
			copiedEventObject['className'] = [$categoryClass];
		// render the event on the calendar
		$this.$calendar.fullCalendar('renderEvent', copiedEventObject, true);
		// is the "remove after drop" checkbox checked?
		if ($('#drop-remove').is(':checked')) {
			// if so, remove the element from the "Draggable Events" list
			eventObj.remove();
		}
	},
	/* on click on event */
	CalendarApp.prototype.onEventClick = function (calEvent, jsEvent, view) {
		var $this = this;
		var form = $("<form></form>");
		form.append("<label>修改事件</label>");
		form.append("<div class='input-group'><input class='form-control' type=text value='" + calEvent.title + "' maxlength='20' /><span class='input-group-btn'><button type='submit' class='btn btn-success waves-effect waves-light'><i class='fa fa-check'></i> 保存</button></span></div>");
		$this.$modal.modal({
			backdrop: 'static'
		});
		$this.$modal.find('.delete-event').show().end().find('.save-event').hide().end().find('.modal-body').empty().prepend(form).end().find('.delete-event').unbind('click').click(function () {
			$this.$calendarObj.fullCalendar('removeEvents', function (ev) {
				if (ev._id == calEvent._id) {
					$.post("/calendar-event-remove", {
						id: ev._id
					}, function (result) {
						if(result.success){
							console.log(result);
						}
					}, "json").fail(function () {
						alert("删除失败，网络错误");
					});
				}

				return (ev._id == calEvent._id);
			});
			$this.$modal.modal('hide');
		});
		$this.$modal.find('form').on('submit', function () {
			calEvent.title = form.find("input[type=text]").val();
			//calEvent.className = form.find("select").val();
			console.log(calEvent);
			$.post("/calendar-event-edit", {
				id: calEvent.id,
				title:calEvent.title
			}, function (result) {
				if(result.success){
					$this.$calendarObj.fullCalendar('updateEvent', result.item);
				}
			}, "json").fail(function () {
				alert("编辑失败，网络错误");
			});
			$this.$modal.modal('hide');
			return false;
		});
	},
	/* on select */
	CalendarApp.prototype.onSelect = function (start, end, allDay) {
		var $this = this;
		$this.$modal.modal({
			backdrop: 'static'
		});
		var form = $("<form></form>");
		form.append("<div class='row'></div>");
		form.find(".row")
		.append("<div class='col-md-10'><div class='form-group'><label class='control-label'>事件名称</label><input class='form-control' required placeholder='请输入事件名称' maxlength='20' type='text' name='title'/></div></div>")
		.append("<div class='col-md-2'><div class='form-group'><label class='control-label'>分类</label><select class='form-control' name='category'></select></div></div>")
		.find("select[name='category']")
		.append("<option value='bg-red'>红</option>")
		.append("<option value='bg-orange'>橙</option>")
		.append("<option value='bg-yellow'>黄</option>")
		.append("<option value='bg-green'>绿</option>")
		.append("<option value='bg-cyan'>青</option>")
		.append("<option value='bg-blue'>蓝</option>")
		.append("<option value='bg-purple'>紫</option>")
		.append("<option value='bg-pink'>粉</option>");

		$this.$modal.find('.delete-event').hide().end().find('.save-event').show().end().find('.modal-body').empty().prepend(form).end().find('.save-event').unbind('click').click(function () {
			form.submit();
		});

		$this.$modal.find('form').on('submit', function () {
			var title = form.find("input[name='title']").val();
			var beginning = form.find("input[name='beginning']").val();
			var ending = form.find("input[name='ending']").val();
			var categoryClass = form.find("select[name='category'] option:checked").val();
			if (title !== null && title.length != 0) {
				$.post("/calendar-event-add", {
					title: title,
					startDate: new Date(start.format("YYYY-MM-DD HH:mm:ss")).getTime(),
					endDate: new Date(end.format("YYYY-MM-DD HH:mm:ss")).getTime(),
					allDay: false,
					className: categoryClass
				}, function (result) {
					if (result.success) {
						$this.$calendarObj.fullCalendar('renderEvent', result.item, true);
					} else {
						alert("创建事件失败");
					}
				}, "json").fail(function () {
					alert("创建事件失败");
				});

				$this.$modal.modal('hide');
			} else {
				alert('请填写事件名称');
			}
			return false;

		});
		$this.$calendarObj.fullCalendar('unselect');
	},
	CalendarApp.prototype.enableDrag = function () {
		//init events
		$(this.$event).each(function () {
			// create an Event Object (http://arshaw.com/fullcalendar/docs/event_data/Event_Object/)
			// it doesn't need to have a start or end
			var eventObject = {
				title: $.trim($(this).text()) // use the element's text as the event title
			};
			// store the Event Object in the DOM element so we can get to it later
			$(this).data('eventObject', eventObject);
			// make the event draggable using jQuery UI
			$(this).draggable({
				zIndex: 999,
				revert: true, // will cause the event to go back to its
				revertDuration: 0 //  original position after the drag
			});
		});
	}
	/* Initializing */
	CalendarApp.prototype.init = function () {
		this.enableDrag();
		/*  Initialize the calendar  */
		var date = new Date();
		var d = date.getDate();
		var m = date.getMonth();
		var y = date.getFullYear();
		var form = '';
		var today = new Date($.now());

		var defaultEvents = [];
		var taDefaultEvents = $("#taDefaultEvents").val();
		if(taDefaultEvents){
			var calEventObj = JSON.parse(taDefaultEvents);
			defaultEvents = calEventObj.items;
		}
		var $this = this;
		$this.$calendarObj = $this.$calendar.fullCalendar({
				slotDuration: '00:15:00', /* If we want to split day time each 15minutes */
				minTime: '08:00:00',
				maxTime: '19:00:00',
				defaultView: 'month',
				handleWindowResize: true,
				header: {
					left: 'prev,next today',
					center: 'title',
					right: 'month,agendaWeek,agendaDay'
				},
				events: defaultEvents,
				editable: true,
				droppable: true, // this allows things to be dropped onto the calendar !!!
				eventLimit: true, // allow "more" link when too many events
				selectable: true,
				drop: function (date) {
					$this.onDrop($(this), date);
				},
				select: function (start, end, allDay) {
					$this.onSelect(start, end, allDay);
				},
				eventClick: function (calEvent, jsEvent, view) {
					$this.onEventClick(calEvent, jsEvent, view);
				},
				eventAfterAllRender: function (evt, b, c) {
					console.log(evt.getCurrentEvents());
				}
			});

		//on new event
		this.$saveCategoryBtn.on('click', function () {
			var categoryName = $this.$categoryForm.find("input[name='category-name']").val();
			var categoryColor = $this.$categoryForm.find("select[name='category-color']").val();
			if (categoryName !== null && categoryName.length != 0) {
				$this.$extEvents.append('<div class="calendar-events bg-' + categoryColor + '" data-class="bg-' + categoryColor + '" style="position: relative;"><i class="fa fa-move"></i>' + categoryName + '</div>')
				$this.enableDrag();
			}
		});
	},

	//init CalendarApp
	$.CalendarApp = new CalendarApp,
	$.CalendarApp.Constructor = CalendarApp

}
(window.jQuery),

//initializing CalendarApp
function ($) {
	"use strict";
	$.CalendarApp.init()
}
(window.jQuery);
