/*====================================================
  TABLE OF CONTENT
  1. function declearetion
  2. Initialization
====================================================*/

/*===========================
 1. function declearetion
 ==========================*/
var themeApp = {
	darkScheme: function() {
		if(typeof dark_color_scheme !== 'undefined' && dark_color_scheme == true) {
			$('body').addClass('dark-bg');
		}
	},
	featuredMedia: function(){
		$(".post-wrap").each(function() {
			var thiseliment = $(this);
			var container = $(this).find('.card-header');
			var media_wrapper = $(this).find('featured');
			var media_content_embeded = media_wrapper.find('iframe');
			if (media_content_embeded.length > 0) {
				$(container).html($(media_content_embeded)).wrapInner("<div class='featured-image-countainer'></div>");
				thiseliment.addClass('embeded-media');
			}
		});
	},
	responsiveIframe: function() {
		$('.card').fitVids();
	},
	navScroll: function() {
		$(window).scroll(function() {
			if($(this).scrollTop() > 0) {
				$('.navbar-fixed-top').addClass('page-scrolled');
			} else {
				$('.navbar-fixed-top').removeClass('page-scrolled');
			}
		});
	},
	navStyle: function() {
		if(typeof fixed_navbar !== 'undefined' && fixed_navbar == false) {
			$('body').removeClass('fixed-navbar');
			$('.navbar-default').removeClass('navbar-fixed-top');
		}
	},
	shareToggle: function() {
		$('.share-button').on('click', function(){
			$(this).siblings('.share-link-wrap').toggleClass('opened');
		})
	},
	gatherData:function(page, prevPagePostID, data) {
		var page = page || 1;
		var prevPagePostID = prevPagePostID || '';
		var data = data || [];
		var parent = this;
		var status = '';

		var FEED_URL = '/rss/' + page + '/';

		$.ajax({
			url: FEED_URL,
			type: 'get',
			success: function(parsedData) {
				var currPagePostID = $(parsedData).find('item > guid').text();
				if(prevPagePostID != currPagePostID) {
					$(parsedData).find('item').each(function(){
						data.push(this);
					});
					parent.gatherData(page+1, currPagePostID, data);
				} else {
					parent.tagcloud(data);
					parent.recentPosts(data);
				}
			}
		});
	},
	tagcloud:function(data){
		var primary_array = [];
		$(data).find("category").each(function () {
			var el = $(this).text();
			if ($.inArray(el, primary_array) == -1) {
				primary_array.push(el);
			}
		});
		var formated_tag_list = "";
		for ( var i = 0; i < primary_array.length; i = i + 1 ) {
			var tag = primary_array[ i ];
			var tagLink = tag.toLowerCase().replace(/ /g, '-');
			formated_tag_list += ("<a href=\"/tag/" + tagLink + "\">" + tag + "</a>");
		}
		$('.tag-cloud').append(formated_tag_list);
	},
	recentPosts: function(data) {
		var container = $(".recent-post");
		if(container.length && typeof recent_post_count !== 'undefined') {
			var string = '';
			$(data).slice(0,recent_post_count).each(function() {
				var link = $(this).find('link').text();
				var title = $(this).find('title').text();
				var published_date = themeApp.formatDate($(this).find('pubDate').text());
				var image_link  = $(this).find('media\\:content, content').attr('url');
				if (typeof image_link !== 'undefined') {
					var image = '<div class="post-thumb pull-left" style="background-image:url(' + image_link + ')"></div>';
					var helper_class = 'have-image';
				} else {
					var image ='<div class="post-thumb pull-left"><i class="fa fa-image"></i></div>';
					var helper_class = '';
				}
				string +='<div class="recent-single-post clearfix ' +helper_class+ '"><a href="' + link + '" class="post-title">\
				'+ image +'\
				<div class="post-info"><h4 class="h5">' + title + '</h4><div class="date"><i class="material-icons">&#xE192;</i>' + published_date + '</div></div>\
				</a></div>'
			});
			container.append(string);
		}
	},
	formatDate: function(dt) {
		var d = new Date(dt);
		var month_name = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
		var month = month_name[d.getMonth()];
		var date = d.getDate();
		var year = d.getFullYear();
		var formatted_dt = month+' '+date+','+' '+year;
		return formatted_dt;
	},
	matrerialInput: function() {
		
	},
	mailchimp:function() {
		function IsEmail(email) {
			var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
			return regex.test(email);
		}
	},
	highlighter: function() {
		$('pre code').each(function(i, block) {
		    hljs.highlightBlock(block);
		});
	},
	searchPopup: function() {
		$('.search-toggle').on('click', function(e) {
			e.preventDefault();
			$('.search-popup').addClass('visible');
			$('#search-input').css('visibility', 'visible').focus();
		});
		$('.close-button').on('click', function(e) {
			e.preventDefault();
			$('.search-popup').removeClass('visible');
		});
	},
	SearchProcess: function() {
		$('#search-input').ghostHunter({
			results: "#search-results",
			displaySearchInfo   : false,
			onKeyUp: true,
			result_template : '<div class="result-item"><a href="{{link}}"><i class="material-icons">&#xE145;</i>{{title}}</a></div>',
		});
	},
	clickRipple: function() {
		$('.ripple').on('click', function (event) {
			// event.preventDefault();
			console.log($(this).height());
			var $div = $('<span/>');
			var btnOffset = $(this).offset();
			var xPos = event.pageX - btnOffset.left;
			var yPos = event.pageY - btnOffset.top;
			console.log(xPos, yPos);
			$div.addClass('ripple-effect');
			var $ripple = $(".ripple-effect");
			$ripple.css("height", $(this).height());
			$ripple.css("width", $(this).height());
			$div.css({
				top: yPos - ($ripple.height()/2),
				left: xPos - ($ripple.width()/2),
				background: $(this).data("ripple-color")
			}).appendTo($(this));
			window.setTimeout(function(){
			$div.remove();
			}, 1000);
		});
	},
	backToTop: function() {
		$(window).scroll(function(){
			if ($(this).scrollTop() > 100) {
				$('#back-to-top').fadeIn();
			} else {
				$('#back-to-top').fadeOut();
			}
		});
		$('#back-to-top').on('click', function(e){
			e.preventDefault();
			$('html, body').animate({scrollTop : 0},1000);
			return false;
		});
	},

	init: function() {
		themeApp.darkScheme();
		themeApp.featuredMedia();
		themeApp.responsiveIframe();
		themeApp.navStyle();
		themeApp.navScroll();
		themeApp.shareToggle();
		themeApp.gatherData();
		themeApp.matrerialInput();
		themeApp.mailchimp();
		themeApp.highlighter();
		themeApp.searchPopup();
		themeApp.SearchProcess();
		themeApp.clickRipple();
		themeApp.backToTop();
	}
}

/*===========================
2. Initialization
==========================*/
$(document).ready(function(){
	themeApp.init();
	
	$('.btn-close').click(function () {
		$(this).parents('.dialog').addClass('hide');
	});
});

function popResult(txt) {
    var msg = $('<div class="msg hide">' + txt + '</div>');
    msg.css('left', '50%');
    $('body').append(msg);
    msg.css('margin-left', '-' + parseInt(msg.outerWidth() / 2) + 'px');
    msg.removeClass('hide');
    setTimeout(function () {
        msg.addClass('hide');
        setTimeout(function () {
            msg.remove();
        }, 400);
    }, 2600);
}