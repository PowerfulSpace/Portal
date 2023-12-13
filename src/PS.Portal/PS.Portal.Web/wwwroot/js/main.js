$(function() {

	$('select[multiple].active.3col').multiselect({
	  columns: 3,
	  placeholder: 'Select States',
	  search: true,
	  searchOptions: {
	      'default': 'Search States'
	  },
	  selectAll: true
	});

});