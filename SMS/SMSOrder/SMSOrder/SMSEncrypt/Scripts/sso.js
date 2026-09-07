(function() {

	var SETTINGS = {};
	var SSO_SERVER_URL = '/';
	var SSO_URL_API_PRELOGIN = SSO_SERVER_URL + 'api/prelogin';
	var SSO_URL_API_LOGIN = SSO_SERVER_URL + 'api/login';
	var SSO_URL_API_LOGOUT = SSO_SERVER_URL + 'api/logout';
	var SSO_URL_API_BEETALK_PRELOGIN = SSO_SERVER_URL + 'api/beetalk/prelogin';
	var SSO_URL_API_BEETALK_LOGIN = SSO_SERVER_URL + 'api/beetalk/login';
	var SSO_URL_API_BEETALK_SEND_OTP = SSO_SERVER_URL + 'api/beetalk/send_otp';
	var SSO_URL_API_BEETALK_LOGIN_BY_OTP = SSO_SERVER_URL+ 'api/beetalk/login_by_otp';
	var SSO_URL_API_FACEBOOK_PLATFORM_LOGIN = SSO_SERVER_URL + 'api/facebook/platform_login';
	var SSO_URL_UI_REGISTER = SSO_SERVER_URL + 'ui/register';
	var SSO_URL_API_AUTH = SSO_SERVER_URL + 'oauth/token/grant';
	var SSO_URL_OAUTH_TOKEN_FACEBOOK_EXCHANGE = SSO_SERVER_URL + 'oauth/token/facebook/exchange';
	var SSO_URL_OAUTH_TOKEN_VK_EXCHANGE = SSO_SERVER_URL + 'oauth/token/vk/exchange/v2';
	var SSO_URL_OAUTH_TOKEN_LINE_EXCHANGE = SSO_SERVER_URL + 'oauth/token/line/exchange';
	var SSO_URL_OAUTH_TOKEN_GOOGLE_EXCHANGE = SSO_SERVER_URL + 'oauth/token/google/exchange';
	var SSO_URL_OAUTH_TOKEN_HUAWEI_EXCHANGE = SSO_SERVER_URL + 'oauth/token/huawei/exchange';
	var SSO_URL_API_REG = SSO_SERVER_URL + 'api/register';
	var SSO_URL_API_REG_PREPARE = SSO_URL_API_REG + '/prepare';
	var SSO_URL_API_REG_CHECK = SSO_URL_API_REG + '/check';
	var DEFAULT_REDIRECT_URL = 'http://www.garena.com/';
	var FACEBOOK_OAUTH_URL = 'https://www.facebook.com/dialog/oauth';
	var VK_OAUTH_URL = 'https://oauth.vk.com/authorize';
	var GOOGLE_OAUTH_URL = 'https://accounts.google.com/o/oauth2/v2/auth';
	var LINE_OAUTH_URL = 'https://access.line.me/dialog/oauth/weblogin';
	var HUAWEI_OAUTH_URL = 'https://oauth-login.cloud.huawei.com/oauth2/v2/authorize';
	var GAS_APP_URL = 'gas://';
	var GAS_IOS = 'http://itunes.apple.com/app/id857669215';
	var GAS_ANDROID = 'http://cdn.garenanow.com/gas/mobile/android/gas.apk';
	var CAPTCHA_SERVICE = 'https://gop.captcha.garena.com/image';
	var DEFAULT_LOCALE = 'en_SG';
	var ACCOUNT_CENTER_URL = 'https://account.garena.com';
	var ACCOUNT_CENTER_TEST_URL = 'https://account.test.garena.com';
	var ACCOUNT_CENTER_RECOVERY_URL = 'https://account.garena.com/recovery';
	var ACCOUNT_CENTER_RECOVERY_TEST_URL = 'https://account.test.garena.com/recovery';
	var FB_PLATFORM_MODE = 'platform';
	var GOOGLE_COOKIE_NAME = 'googleLoginParam'

	var KEY_CODE_ENTER = 13;
	var PLATFORM_GARENA = 1;
	var PLATFORM_BEETALK = 2;
	var PLATFORM_FACEBOOK = 3;
	var PLATFORM_VK = 5;
	var PLATFORM_LINE = 6;
	var PLATFORM_HUAWEI = 7;
	var PLATFORM_GOOGLE = 8;

	var OTP_SMS_INTERVAL = 60;
	var OTP_REGISTER_INTERVAL = 30;

	var captcha_key = '';
	var mobile_register_request = {};

	var inIframe = function () {
		try {
		return window.self !== window.top;
		} catch (e) {
		return true;
		}
	}();

	function isMobile(){
		return /android|ip(hone|ad|od)/i.test(navigator.userAgent);
	}

	function _(key, params) {
		var language = SETTINGS['language'];
		//if(!(language in SSO_SERVER_I18N) || !(key in SSO_SERVER_I18N[language])) {
		//	return key;
		//}
		//var text = SSO_SERVER_I18N[language][key];
		//if(params != null) {
		//	for(key in params) {
		//		text = text.replace('%(' + key + ')', params[key].toString());
		//	}
		//}
        return key;
	}

	function getLocale(language, country) {
		if (language=='zh-TW') {
			return language
		} else {
			return language + '-' + country
		}
	}

	function getCurrentBaseUrl() {
		var url = window.location.href;
		var sep = url.indexOf('#');
		if(sep >= 0) {
			url = url.substring(0, sep);
		}
		sep = url.indexOf('?');
		if(sep >= 0) {
			url = url.substring(0, sep);
		}
		return url;
	}

	function getRequestParams() {
		var e = window.location.search.replace(/^\?/, "").split("&"), n = 0, r, i = {}, s, o, u;
		while(( r = e[n++]) !== undefined) s = r.match(/^([^=&]*)=(.*)$/), s && ( o = decodeURIComponent(s[1]), u = decodeURIComponent(s[2]), i[o] = u);
		return i;
	}

	function getRequestParam(key) {
		var params = getRequestParams();
		if(key in params) {
			return params[key];
		}
		return null;
	}

	function getRequestFragments() {
		var e = window.location.hash.replace(/^#/, "").split("&"), n = 0, r, i = {}, s, o, u;
		while(( r = e[n++]) !== undefined) s = r.match(/^([^=&]*)=(.*)$/), s && ( o = decodeURIComponent(s[1]), u = decodeURIComponent(s[2]), i[o] = u);
		return i;
	}

	function getRequestFragment(key) {
		var params = getRequestFragments();
		if(key in params) {
			return params[key];
		}
		return null;
	}

	function redirect(uri, params) {
		if(params != null) {
			var sParams = '';
			for(var key in params) {
				sParams += '&' + encodeURIComponent(key) + '=' + encodeURIComponent(params[key]);
			}
			if(sParams.length > 0) {
				if(uri.indexOf('?') < 0) {
					sParams = '?' + sParams.substr(1);
		 		}
	 			uri += sParams;
			}
 		}
		window.location = uri;
	}

	function setCookie(name, value, expiredSeconds, domain) {
		var sValue = escape(value);
		if(expiredSeconds != null) {
			var expires = new Date(new Date().getTime() + expiredSeconds);
			sValue += "; expires=" + expires.toUTCString();
		}
		if(domain != null) {
			sValue += "; domain=" + domain;
		}
		document.cookie = name + "=" + sValue;
	}

	function removeCookie(name, domain) {
		document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:01 GMT; domain=' + domain + '; path=/;';
	}

	function appendClearDiv(container) {
		container.append($('<div/>', {'class' : 'clear'}));
	}

	function clearMessage(parent) {
		$(parent + ' #msg').remove();
		$(parent+' input.error').removeClass('error');
	}

	function showMessage(msg, type, parent, clear_others, mode) {
		mode = (typeof mode === "undefined") ? "text" : mode;
		if(clear_others){
			$('span[id="msg"]').remove();
		} else {
			parent.find('span.errorMsg,span.successMsg').remove();
		}
		var panel = $('<span/>', {'id': 'msg', 'class': type + 'Msg'});
		if(mode=='html'){
			panel.html('<em></em>'+msg);
		} else {
			panel.text(msg);
			panel.prepend($('<em/>'));
		}
		parent.append(panel);
	}

	function showMobileRegisterAlert(msg) {
		var wrapper = $('<div/>').addClass('alertWrapper');
		var container = $('<div/>').addClass('alertContainer').appendTo(wrapper);
		var alert = $('<div/>').addClass('alert').appendTo(container);
		$('<span/>').addClass('icon-alert').appendTo(alert);
		$('<p/>').text(msg).appendTo(alert);
		$('<p/>').text(_('register_redirect_on_error')).appendTo(alert);
		var btnOK = $('<a/>', {'href': 'javascript:;', 'class': 'btn-ok'}).text(_('btn_ok')).appendTo(container);

		wrapper.appendTo($('body'));

		btnOK.click(function(){
			showRegisterPage('mobile');
			wrapper.remove();
		})
	}

	function showSuccess(msg, after, clear_others, mode) {
		showMessage(msg, 'success', after, clear_others, mode);
	}

	function showError(msg, after, clear_others, mode) {
		if(after===undefined){after=$('#line-btn');}
		after.find('span.icon-right').remove();
		clear_others = (typeof clear_others === "undefined") ? true : clear_others;
		if (clear_others) {
			$('input.error').removeClass('error');
		}
		showMessage(msg, 'error', after, clear_others, mode);
		after.find('input').addClass('error');
	}

	function requestJsonp(url, params, callback) {
		params['format']  = 'jsonp';
		params['callback'] = callback;
		params['id']= new Date().getTime();
		params['app_id'] = SETTINGS['app_id'];
		var first = true;
		for(var key in params) {
			var value = params[key];
			if(first) {
				url += '?';
				first = false;
			} else {
				url += '&';
			}
			url += encodeURIComponent(key);
			url += '=';
			url += encodeURIComponent(value);
		}
		var scriptTag = $('<script/>', {'type': 'text/javascript'}).appendTo($('body'));
		scriptTag.attr('src', url);
	}

	function requestJson(url, params, callback, method) {
		method = (typeof method === "undefined") ? "GET" : method;
		params['format']  = 'json';
		params['id']= new Date().getTime();
		params['app_id'] = SETTINGS['app_id'];
		$.ajax({
			type : method,
			url : url,
			data : params,
			dataType : 'json',
			success : callback,
			error : function() {
				callback({'error': 'error_server'});
			}
		});
	}

	function checkPasswordValid(username, password) {
		if(password.length < 8 || password.length > 16) {
			return false;
		}
		if(password === username) {
			return false;
		}

		var has_small_letter = false;
		var has_capital_letter = false;
		var has_number = false;
		var has_symbol = false;
		for(var i=0; i < password.length; i++) {
			var code = password.charCodeAt(i);
			var ch = password.charAt(i);
			if(code > 126 || code < 33) {
				return false;
			}
			if(ch >= 'a' && ch <= 'z') {
				has_small_letter = true;
			} else if(ch >= 'A' && ch <= 'Z') {
				has_capital_letter = true;
			} else if(ch >= '0' && ch <= '9') {
				has_number = true;
			} else {
				has_symbol = true;
			}
		}

		var pass_strength = 0;
		if(has_small_letter) {
			pass_strength++;
		}
		if(has_capital_letter) {
			pass_strength++;
		}
		if(has_number) {
			pass_strength++;
		}
		if(has_symbol) {
			pass_strength++;
		}
		return pass_strength >= 2;
	}

	function centralizeContent() {
		var div = $('.content');
		var top = ($(window).height() - div.height()) / 2;
		var header = $('#header');
		if(header.size() > 0) {
			top -= header.height() / 2;
		}
		top = top / 5 * 4 - 50;
		if(top < 0)	{
			top = 0;
		}
		div.css('top', top.toString() + 'px');
	}

	function clearPage() {
		$(window).unbind('resize', centralizeContent);
		$('#page').remove();
	}

	function centralizeDialog() {
		$('.sso_dialog').each(function() {
			var dialog = $(this);
			var top = ($(window).height() - dialog.height()) / 2;
			if(top < 0)	{
				top = 0;
			}
			dialog.css('top', top.toString() + 'px');
		});
	}

	function removeDialog() {
		$(window).unbind('resize', centralizeDialog);
		$('.sso_dialog_container').remove();
	}

	function showDialog(content, id, title, onCloseCallback) {
		removeDialog();
		var container = $('<div/>').addClass('sso_dialog_container').css('display', 'none');
		var background = $('<div/>').addClass('sso_dialog_background').appendTo(container);
		var dialog = $('<div/>').addClass('sso_dialog').appendTo(container);
		if(id != null) {
			dialog.attr('id', id);
		}
		if(title == null) {
			title = '';
		}
		$('<div/>').addClass('sso_dialog_title').text(title).appendTo(dialog);
		var btnClose = $('<div/>', {'title': _('dialog_close_tip')}).html('&times;').addClass('sso_dialog_close').appendTo(dialog);
		var onClose = function() {
			$(window).unbind('resize', centralizeDialog);
			container.fadeOut(500, onCloseCallback);
		};
		btnClose.click(onClose);
		$('<div/>').addClass('sso_dialog_content').append(content).appendTo(dialog);
		$('body').append(container);
		var top = ($(window).height() - dialog.height()) / 2;
		if(top < 0)	{
			top = 0;
		}
		dialog.css('top', top.toString() + 'px');
		$(window).bind('resize', centralizeDialog);
		container.fadeIn(500, function(){$(this).focus()});
		background.click(onClose);
	}

	function showPageDialog(url, id, title, onCloseDialog) {
		var iframe = $('<iframe/>', {'class': 'sso_dialog_frame', 'src': url, 'frameborder': 0, 'scrolling': 'no'}).load(function() {
			setTimeout(function() {
				try {
					$(iframe).height($(iframe).contents().height());
				} catch (ex) {
					// sink exception for cross-origin iframe
				}
			}, 100);
		});
		showDialog(iframe, id, title, onCloseDialog);
	}

	function showContent(content, mode, showBanner) {
		showBanner = (typeof showBanner === "undefined") ? false : showBanner;
		clearPage();
		var title = content.find('h2').text();
		if(title!=null){
			$('title').val(title);
		}
		var page = $('<div/>').attr('id','page');
		if(getRequestParam('display') != 'embedded') {
			var header = $('<div/>').attr('id', 'header').addClass('header').appendTo(page);
			if (mode=='pc') {
				var langWrapper = $('<div/>').addClass('langWrapper').addClass('fr').appendTo(header);
				var langSelect = $('<select/>').addClass('lang').appendTo(langWrapper);
				for (i in LOCALE_LIST){
					var option = $('<option/>', {
						'value': getLocale(LOCALE_LIST[i].code, LOCALE_LIST[i].region), 'text': LOCALE_LIST[i].lname
					}).appendTo(langSelect);
				}
				$('<span/>').addClass('icon-earth').appendTo(langWrapper);
				langSelect.change(function(){
					var locale = langSelect.val();
					var params = getRequestParams();
					params['locale'] = locale;
					redirect(getCurrentBaseUrl(), params);
				});
				langSelect.val(getLocale(SETTINGS.language, SETTINGS.country));
			}
			var topLink = $('<h1/>');
			var link = $('<a/>').addClass('logo').appendTo(topLink);
			if (SETTINGS.platform == PLATFORM_BEETALK) {
				link.attr({'href': _('beetalk_homepage'), 'target': '_blank'});
				$('<img/>', {'src': SETTINGS['static_root']+'images/header_beetalk.jpg', 'width': 135, 'height': 33}).appendTo(link);
			} else {
				link.attr({'href': _('garena_homepage'), 'target': '_blank'});
				if (!SETTINGS.iframe) {
					$('<img/>', {'src': SETTINGS['static_root'] + 'images/img_garena_logo.png', 'height': 35}).appendTo(link);
				} else {
					$('<span/>', {'id': 'iframe_title'}).appendTo(topLink);
				}
				$('<div/>').addClass('topBarGarena').appendTo(header);
			}
			$('<div/>').addClass('topBar').appendTo(header);
			topLink.appendTo(header);
			if (SETTINGS.platform == PLATFORM_BEETALK) {
				content.addClass('content-beetalk');
				header.addClass('header-beetalk')
			}
		}

		var mainPanel = $('<div/>').attr('id', 'main-panel').appendTo(page);
		if (mode=='mobile'){
			mainPanel.addClass('mobile');
		}
		content.addClass('content');
		// var footer = $('<div/>').attr('id', 'footer').text(_('footer_text')).addClass('footer').appendTo(mainPanel);
		// if (SETTINGS.test && SETTINGS.language=='vi' && /android/i.test(navigator.userAgent)){
		// 	footer.text('');
		// }
		if( SETTINGS['gas_download'] && showBanner){
			var gas = $('<div/>').attr('id', 'gas').addClass('banner').appendTo(content);
			$('<img/>', {'src': SETTINGS['static_root'] + 'images/garena_banner.png'}).appendTo(gas);
			$('<span/>').addClass('bannerText').text(_('gas_intro')).appendTo(gas);
			var btnDownload = $('<a/>', {'href': 'javascript:;'}).addClass('download').appendTo(gas);
			$('<span/>').addClass('icon-phone').appendTo(btnDownload);
			$('<span/>').text(_('gas_dl_btn_text')).appendTo(btnDownload);

			btnDownload.click(function(){
				if( /android/i.test(navigator.userAgent) ) {
					var iframe = document.createElement("iframe");
					iframe.id = 'frame_' + (new Date()).getTime();
					iframe.style.border = "none";
					iframe.style.width = "1px";
					iframe.style.height = "1px";
					document.body.appendChild(iframe);
					var r = document.getElementById(iframe.id);
					var loaded = false;
					r.onload = function () {
						window.location = GAS_ANDROID;
						loaded = true;
					};
					window.addEventListener("pagehide", function () {
						loaded = true;
					}, false);
					if (r === null) {
						window.location = GAS_ANDROID;
					} else {
						r.src = GAS_APP_URL;
					}
					setTimeout(function() {
						if (!loaded) {
							window.location = GAS_ANDROID;
						}
					}, 1000);
				} else {
					setTimeout(function(){
						window.location = GAS_IOS;
					}, 100);
					window.location = GAS_APP_URL;
				}
			});
		}
		mainPanel.prepend(content);

		$('body').append(page);
		centralizeContent();
		$(window).bind('resize', centralizeContent);
	}

	function showCaptcha() {
		var sso_captcha = $('.sso_captcha');
		if(!sso_captcha.is(":visible")) {
			sso_captcha.show();
		}
		refreshCaptcha();
	}

	function uuid() {
		return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
			var r = Math.random()*16|0, v = c == 'x' ? r : (r&0x3|0x8);
			return v.toString(16);
		});
	}

	function refreshCaptcha() {
		captcha_key = uuid().replace(/-/g,'');
		var sso_captcha = $('#sso_captcha_image');
		if(sso_captcha.is(":visible")) {
			$(sso_captcha).find('img').attr('src', CAPTCHA_SERVICE+'?key='+captcha_key);
		}
	}

	function changePlatform(platform) {
		var baseUrl = getCurrentBaseUrl();
		var params = getRequestParams();
		params['platform'] = platform;
		redirect(baseUrl, params);
	}

	function appendPlatformsPanel(container) {
		if((SETTINGS.show_platforms & (~(1<<(SETTINGS.platform-1)))) == 0) {
			return;
		}
		var panel = $('<div/>').addClass('partnerLogin').appendTo(container);
		var promptText = $('<p/>').appendTo(panel);
		if(SETTINGS.platform == PLATFORM_BEETALK) {
			promptText.text(_('login_platform_beetalk_prompt'));
		} else {
			promptText.text(_('login_platform_garena_prompt'));
		}
		var links = $('<div/>').addClass('partnerLink').appendTo(panel);
		if(SETTINGS.platform != PLATFORM_GARENA && (SETTINGS.show_platforms & (1<<(PLATFORM_GARENA-1)))) {
			var linkGarena = $('<a/>', {'href': 'javascript:;'}).addClass('icon-garena').appendTo(links);
			linkGarena.click(function() {
				changePlatform(PLATFORM_GARENA);
			});
		}
		if(SETTINGS.platform != PLATFORM_BEETALK && (SETTINGS.show_platforms & (1<<(PLATFORM_BEETALK-1)))) {
			var linkBeetalk = $('<a/>', {'href': 'javascript:;'}).addClass('icon-beetalk').appendTo(links);
			linkBeetalk.click(function() {
				changePlatform(PLATFORM_BEETALK);
			});
		}
		if(SETTINGS.mode == 'oauth_login' && SETTINGS.platform != PLATFORM_FACEBOOK && (SETTINGS.show_platforms & (1<<(PLATFORM_FACEBOOK-1)))) {
			var linkFacebook = $('<a/>', {'href': 'javascript:;'}).addClass('icon-facebook').appendTo(links);
			linkFacebook.click(loginFacebook);
		}
		if(SETTINGS.mode == 'oauth_login' && SETTINGS.platform != PLATFORM_LINE && (SETTINGS.show_platforms & (1<<(PLATFORM_LINE-1)))) {
			var linkLine = $('<a/>', {'href': 'javascript:;'}).addClass('icon-line').appendTo(links);
			linkLine.click(loginLine);
		}
		if(SETTINGS.mode == 'oauth_login' && SETTINGS.platform != PLATFORM_VK && (SETTINGS.show_platforms & (1<<(PLATFORM_VK-1)))) {
			var linkVk = $('<a/>', {'href': 'javascript:;'}).addClass('icon-vk').appendTo(links);
			linkVk.click(loginVk);
		}
		if(SETTINGS.mode == 'oauth_login' && SETTINGS.platform != PLATFORM_GOOGLE && (SETTINGS.show_platforms & (1<<(PLATFORM_GOOGLE-1)))) {
			var linkGoogle = $('<a/>', {'href': 'javascript:;'}).addClass('icon-google').appendTo(links);
			linkGoogle.click(loginGoogle);
		}
		if(SETTINGS.mode == 'oauth_login' && SETTINGS.platform != PLATFORM_HUAWEI && (SETTINGS.show_platforms & (1<<(PLATFORM_HUAWEI-1)))) {
			var linkHuawei = $('<a/>', {'href': 'javascript:;'}).addClass('icon-huawei').appendTo(links);
			linkHuawei.click(loginHuawei);
		}
	}

	function preloginCallback(data) {
		if('error' in data) {
			if(data['error'] == 'error_require_captcha') {
				showCaptcha();
			} else {
				refreshCaptcha();
				if(data['error'] == 'error_captcha') {
					$('#input-captcha').focus();
					showError(_('login_error_captcha'), $('#sso_captcha_wrap'));
				} else {
					showError(_('login_' + data['error']), $('#line-btn'));
				}
			}
			$('#confirm-btn').attr("disabled", false);
			return;
		}
		var password = $('#sso_login_form_password').val();
		var passwordMd5 = CryptoJS.MD5(password);
		var passwordKey = CryptoJS.SHA256(CryptoJS.SHA256(passwordMd5 + data.v1) + data.v2);
		var encryptedPassword = CryptoJS.AES.encrypt(passwordMd5, passwordKey, {mode: CryptoJS.mode.ECB,padding: CryptoJS.pad.NoPadding});
		encryptedPassword = CryptoJS.enc.Base64.parse(encryptedPassword.toString()).toString(CryptoJS.enc.Hex);

		var loginParams = {
			'account' : data.account,
			'password' : encryptedPassword
		};
		if(SETTINGS['mode'] == 'sso_login') {
			var redirect_uri = getRequestParam('redirect_uri');
			if(redirect_uri != null) {
				loginParams['redirect_uri'] = redirect_uri;
			}
		}
		requestJson(SSO_URL_API_LOGIN, loginParams, SSO_SERVER.loginCallback);
	}

	function loginCallback(data) {
		if('error' in data) {
			if(data['error'] == 'error_require_captcha') {
				showCaptcha();
			} else if (data['error'] == 'error_security_ban'){
				var timeout = 7;
				showError(_('login_error_security_ban', {'second': timeout}), $('#line-btn'));
				var interval_id = setInterval(function(){
					timeout--;
					showError(_('login_error_security_ban', {'second': timeout}), $('#line-btn'));
					if (timeout <= 0){
						clearInterval(interval_id);
						if (inIframe) {
							window.open(data['redirect_uri'], '_blank');
							clearMessage('#line-btn');
						} else {
							redirect(data['redirect_uri']);
						}
					}
				}, 1000)
			} else {
				showError(_('login_' + data['error']), $('#line-btn'));
				refreshCaptcha();
			}
			$('#confirm-btn').attr("disabled", false);
			return;
		}
		SETTINGS['login'] = true;
		SETTINGS['uid'] = data['uid'];
		SETTINGS['username'] = data['username'];
		SETTINGS['timestamp'] = data['timestamp'];
		if(SETTINGS['mode'] == 'sso_login') {
			if (!iframeTryPostAfterLogin(data)) {
				if ('redirect_uri' in data) {
					redirect(data['redirect_uri']);
				} else {
					redirect(DEFAULT_REDIRECT_URL, {'session_key': data['session_key']});
				}
			}
		} else if(SETTINGS['mode'] == 'oauth_login') {
			requestJson(SSO_URL_API_AUTH, getRequestParams(), SSO_SERVER.grantTokenCallback, 'POST');
		}
	}

	function beetalkPreloginCallback(data) {
		if('error' in data) {
			$('#confirm-btn').attr("disabled", false);
			if(data['error'] == 'error_require_captcha') {
				showCaptcha();
				return;
			} else if(data['error'] == 'error_captcha') {
				showError(_('register_error_captcha'), $('#line-captcha'), false);
			} else {
				showError(_('beetalk_login_' + data['error']), $('#line-btn'));
			}
			refreshCaptcha();
			return;
		}
		var password = $('#sso_login_form_password').val();
		password = CryptoJS.SHA256(CryptoJS.MD5(password).toString(CryptoJS.enc.Hex)).toString(CryptoJS.enc.Hex);
		password = CryptoJS.SHA256(CryptoJS.MD5(password + data.v1).toString(CryptoJS.enc.Hex)).toString(CryptoJS.enc.Hex);
		password = CryptoJS.SHA256(password + data.v2).toString(CryptoJS.enc.Hex);
		var loginParams = {
			'account' : data.account,
			'password' : password
		};
		if(SETTINGS['mode'] == 'sso_login') {
			var redirect_uri = getRequestParam('redirect_uri');
			if(redirect_uri != null) {
				loginParams['redirect_uri'] = redirect_uri;
			}
		}
		requestJson(SSO_URL_API_BEETALK_LOGIN, loginParams, SSO_SERVER.beetalkLoginCallback);
	}

	function beetalkLoginCallback(data) {
		if('error' in data) {
			if(data['error'] == 'error_require_captcha') {
				showCaptcha();
			} else {
				showError(_('beetalk_login_' + data['error']), $('#line-btn'));
				refreshCaptcha();
			}
			$('#confirm-btn').attr("disabled", false);
			return;
		}
		SETTINGS['login'] = true;
		SETTINGS['uid'] = data['uid'];
		SETTINGS['timestamp'] = data['timestamp'];

		if(SETTINGS['mode'] == 'sso_login') {
			if('redirect_uri' in data) {
				redirect(data['redirect_uri']);
			} else {
				redirect(DEFAULT_REDIRECT_URL, {'session_key': data['session_key']});
			}
		} else if(SETTINGS['mode'] == 'oauth_login') {
			var params = getRequestParams();
			params['grant'] = 1;
			requestJson(SSO_URL_API_AUTH, params, SSO_SERVER.grantTokenCallback, 'POST');
		}
	}

	function beetalkSendOtpCallback(data) {
		if('error' in data) {
			showError(_('beetalk_send_otp_' + data['error']), $('#line-btn'));
		} else {
			showSuccess(_('beetalk_send_otp_info'), $('#line-btn'));
			var leftSeconds = OTP_SMS_INTERVAL;
			var btnSend = $('#send-btn');
			btnSend.attr("disabled", true);
			btnSend.text(_('beetalk_otp_send_btn_timer', {'second': leftSeconds}));
			var timer = setInterval(function () {
				leftSeconds--;
				if(leftSeconds <= 0) {
					clearInterval(timer);
					btnSend.text(_('beetalk_otp_send_btn_resend_sms')).attr("disabled", false);
				} else {
					btnSend.text(_('beetalk_otp_send_btn_timer', {'second': leftSeconds}));
				}
			}, 1000);
		}
	}

	function beetalkOtpLoginCallback(data) {
		if('error' in data) {
			showError(_('beetalk_otp_login_' + data['error']), $('#line-btn'));
			$('#confirm-btn').attr("disabled", false);
			return;
		}
		SETTINGS['login'] = true;
		SETTINGS['uid'] = data['uid'];
		SETTINGS['timestamp'] = data['timestamp'];

		if(SETTINGS['mode'] == 'sso_login') {
			if('redirect_uri' in data) {
				redirect(data['redirect_uri']);
			} else {
				redirect(DEFAULT_REDIRECT_URL, {'session_key': data['session_key']});
			}
		} else if(SETTINGS['mode'] == 'oauth_login') {
			var params = getRequestParams();
			params['grant'] = 1;
			requestJson(SSO_URL_API_AUTH, params, SSO_SERVER.grantTokenCallback, 'POST');
		}
	}

	function grantTokenCallback(data) {
		if('error' in data) {
			if(data['error'] == 'error_require_grant') {
				if (isMobile()){
					showGrantPage('mobile');
				} else {
					showGrantPage('pc');
				}
				return;
			}
			if (data['error'] == 'error_user_ban') {
				showError(_('login_error_user_ban'));
			} else {
				showError(_('grant_error'));
			}
			removeCookie(SETTINGS.session_cookie_name, SETTINGS.session_cookie_domain);
			$('#confirm-btn').attr("disabled", false);
			return;
		}
		redirect(data['redirect_uri']);
	}

	function showRegisterPage(mode) {
		var panelRegister = $('<div/>');
		$('<h2/>').addClass('title').text(_('register_header')).appendTo(panelRegister);

		var formRegister = $('<form/>', {'id': 'form-register'}).addClass('loginForm').appendTo(panelRegister);
		var txtUsername = $('<input/>', {
			'id': 'sso_register_form_account', 'type': 'text', 'placeholder': _('register_form_username_prompt'), 'autocorrect': 'off', 'autocapitalize': 'off'
		}).appendTo($('<div/>', {'id': 'line-username'}).addClass('line').appendTo(formRegister)).val(mobile_register_request['username']);
		var txtPassword = $('<input/>', {
			'id': 'sso_register_form_password', 'type': 'password', 'placeholder': _('register_form_password_prompt')
		}).appendTo($('<div/>', {'id': 'line-password'}).addClass('line').appendTo(formRegister)).val(mobile_register_request['password']);
		var togglePassword = $('<span/>').addClass('icon-eye icon-eye-hide fr');
		txtPassword.after($('<span/>', {'id': 'span-toggle', 'class': 'icon clearfix'}).append(togglePassword));
		togglePassword.click(function(){
			if (togglePassword.hasClass('icon-eye-hide')){
				togglePassword.removeClass('icon-eye-hide');
				txtPassword.attr('type', 'text');
			} else {
				togglePassword.addClass('icon-eye-hide');
				txtPassword.attr('type', 'password');
			}
		});

		if (mode == 'pc') {
			var txtPasswordConfirm = $('<input/>', {
				'id': 'sso_register_form_password_confirm', 'type': 'password', 'placeholder': _('register_form_confirm_password_prompt')
			}).appendTo($('<div/>', {'id': 'line-password-confirm', 'class': 'line'}).appendTo(formRegister));
		}

		var lineEmail = $('<div/>', {'id': 'line-email'}).addClass('line').appendTo(formRegister);
		var txtEmail = $('<input/>', {
			'id': 'sso_register_form_email', 'type': 'email', 'placeholder': _('register_form_email_prompt'), 'autocorrect': 'off', 'autocapitalize': 'off'
		}).appendTo(lineEmail).val(mobile_register_request['email']);
		var txtEmailReminder = $('<div/>', {'class': 'reminder'}).text(_('register_email_reminder')).appendTo(lineEmail);

		var selectCountry = $('<select/>', {
			'id': 'sso_register_form_select_country', 'name': 'select_country'
		}).appendTo($('<div/>', {'id': 'line-select-country'}).addClass('line').appendTo(formRegister));
		for (i in COUNTRY_LIST) {
			var option = $('<option/>', {'value': COUNTRY_LIST[i].code});
			var content = COUNTRY_LIST[i].name + " ";
			if (COUNTRY_LIST[i].hasOwnProperty("lname")) {
				content = content + "(" + COUNTRY_LIST[i].lname + "\u200e) ";
			}
			$('<p/>').text(content).appendTo(option);
			selectCountry.append(option);
		}
		// if (mode == 'mobile') {
		// 	var mobileLine = $('<div/>', {'id': 'line-mobile-number', 'class': 'line phoneLine'}).appendTo(formRegister);
		// 	var spanCountryCode = $('<span/>', {'id': 'country-code', 'class': 'countryCode'}).appendTo(mobileLine);
		// 	var txtMobile = $('<input/>', {
		// 		'id': 'sso_register_form_mobile_number', 'type': 'tel', 'placeholder': _('register_form_mobile_number_prompt')
		// 	}).appendTo(mobileLine).val(mobile_register_request['mobile_no2']);
		// 	selectCountry.change(function () {
		// 		for (i in COUNTRY_LIST) {
		// 			if (COUNTRY_LIST[i]['code'] == this.value) {
		// 				spanCountryCode.text('+' + COUNTRY_LIST[i]['pcode']);
		// 				break;
		// 			}
		// 		}
		// 	})
		// }
		// if (mobile_register_request['country']) {
		// 	selectCountry.val(mobile_register_request['country']).change();
		// } else {
		selectCountry.val(SETTINGS.country).change();
		// }

		var captchaWrapper = $('<div/>', {'class': 'line clearfix', 'id': 'sso_captcha_wrap'}).appendTo(formRegister);
		var rowCaptcha1 = $('<div/>', {'id': 'line-captcha', 'class': 'sso_captcha'}).appendTo(captchaWrapper);
		$('<input/>', {
			'id': 'input-captcha', 'name': 'captcha', 'type': 'tel', 'placeholder': _('login_form_captcha_prompt'), 'autocorrect': 'off', 'autocapitalize': 'off'
		}).appendTo(rowCaptcha1);
		var rowCaptcha2 = $('<div/>', {'id': 'sso_captcha_image', 'class': 'clearfix sso_captcha'}).appendTo(captchaWrapper);
		$('<img/>').appendTo($('<span/>', {'class': 'code fl'}).appendTo(rowCaptcha2));
		var refresh_captcha = $('<a/>', {'href': 'javascript:;', 'class': 'refresh fl'}).text(_('refresh_captcha')).appendTo(rowCaptcha2);
		refresh_captcha.click(refreshCaptcha);
		rowCaptcha1.hide();
		rowCaptcha2.hide();

		var terms_div = $('<div/>', {'id': 'line-terms', 'class': 'terms line'});

		if (mode == 'pc') {
			var checkbox = $('<a/>', {
				'href': 'javascript:;', 'class': 'checkbox'
			}).appendTo(terms_div);
		}
		var terms_label = $('<label/>', {
			'id': 'sso_register_form_terms_label', 'for': 'sso_register_form_terms'
		}).appendTo(terms_div);
		var terms_link = $('<a/>', {
			'id':'sso_register_terms_link', 'text': _('register_form_terms_link_text'), 'href': 'javascript:;'
		});
		var privacy_link = $('<a/>', {
			'id':'sso_register_privacy_link', 'text': _('register_form_privacy_link_text'), 'href': 'javascript:;'
		});
		var ban_rules_link = $('<a/>', {
			'id':'sso_register_ban_rules_link', 'text': _('register_form_ban_rules_link_text'), 'href': 'javascript:;'
		});
		if (mode=='pc') {
			if (SETTINGS.country == 'TW') {
				terms_label.html(_('register_form_terms_label_pc',
					{'tos': $('<div/>').append(terms_link).html(), 'pp': $('<div/>').append(privacy_link).html(), 'br': $('<div/>').append(ban_rules_link).html()}
				));
			} else {
				terms_label.html(_('register_form_terms_label_pc',
					{'tos': $('<div/>').append(terms_link).html(), 'pp': $('<div/>').append(privacy_link).html()}
				));
			}
		} else {
			if (SETTINGS.country == 'TW') {
				terms_label.html(_('register_form_terms_label_mobile',
					{'tos': $('<div/>').append(terms_link).html(), 'pp': $('<div/>').append(privacy_link).html(), 'br': $('<div/>').append(ban_rules_link).html()}
				));
			} else {
				terms_label.html(_('register_form_terms_label_mobile',
					{'tos': $('<div/>').append(terms_link).html(), 'pp': $('<div/>').append(privacy_link).html()}
				));
			}
		}

		if (mode=='pc'){
			terms_div.appendTo(formRegister);
		}

		var btnRegister = $('<input/>', {
			'id': 'confirm-btn', 'type': 'submit', 'value': _('register_form_button_register'), 'onClick': 'javascript:return false;'
		}).addClass('btn').appendTo($('<div/>', {'id': 'line-btn'}).addClass('btnLine').appendTo(formRegister));

		if (mode=='mobile'){
			terms_div.appendTo(formRegister);
		}

		showContent(panelRegister, mode);

		$('#sso_register_privacy_link').click(function () {
			var url = _('register_privacy_url');
			if (!url.match('^http')) {
				url = SETTINGS['static_root'] + url;
			}
			showPageDialog(url, 'sso_register_pp_dialog', _('register_privacy_dialog_title'));
			$('#sso_register_pp_dialog .sso_dialog_content').height(Math.floor($(window).height() * 0.9 - 80));
			centralizeDialog();
		});
		$('#sso_register_terms_link').click(function () {
			var url = _('register_terms_url');
			if (!url.match('^http')) {
				url = SETTINGS['static_root'] + url;
			}
			showPageDialog(url, 'sso_register_tos_dialog', _('register_terms_dialog_title'));
			$('#sso_register_tos_dialog .sso_dialog_content').height(Math.floor($(window).height() * 0.9 - 80));
			centralizeDialog();
		});
		$('#sso_register_ban_rules_link').click(function () {
			var url = _('register_ban_rules_url');
			if (!url.match('^http')) {
				url = SETTINGS['static_root'] + url;
			}
			showPageDialog(url, 'sso_register_tos_dialog', _('register_terms_dialog_title'));
			$('#sso_register_tos_dialog .sso_dialog_content').height(Math.floor($(window).height() * 0.9 - 80));
			centralizeDialog();
		});

		/* validation */
		function validateEmail(is_submit){
 			if(!txtEmail.val()){
 				clearMessage('#line-email');
 				$('#line-email').find('span.icon-right').remove();
 				if(is_submit===true){return 0;}
			} else if(txtEmail.val().length > 50) {
				showError(_('register_error_email_length'), $('#line-email'), false);
			} else if (!/^[\w\-+.%]+@[\w\-.]+\.\w+$/.test(txtEmail.val())) {
				showError(_('register_error_email_invalid'), $('#line-email'), false);
			} else {
				if(is_submit===true){return 0;}
				requestJson(SSO_URL_API_REG_CHECK, {'email' : txtEmail.val()}, function(data) {
					if ('error' in data) {
						if (data['error'] == 'error_email_existed') {
							showError(_('register_error_email_existed'), $('#line-email'), false);
						}
					} else {
						clearMessage('#line-email');
						$('<span/>').addClass('icon-right').appendTo($('#line-email'));
					}
				})
			}
		}
		function validateUsername(is_submit) {
			if(!txtUsername.val()){
				showError(_('register_error_username_empty'), $('#line-username'), false);
			} else if(txtUsername.val().length < 6 || txtUsername.val().length > 15) {
				showError(_('register_error_username_length'), $('#line-username'), false);
			}else {
				if(is_submit===true){return 0;}
				requestJson(SSO_URL_API_REG_CHECK, {'username' : txtUsername.val()}, function(data) {
					if ('error' in data) {
						if (data['error'] == 'error_username_existed') {
							showError(_('register_error_username_existed'), $('#line-username'), false);
						} else if (data['error'] == 'error_username_invalid') {
							showError(_('register_error_username_invalid'), $('#line-username'), false);
						}
					} else {
						clearMessage('#line-username');
						$('<span/>').addClass('icon-right').appendTo($('#line-username'));
					}
				})
			}
		}
		function validatePassword(){
			if (mode=='pc' && txtPassword.val() != txtPasswordConfirm.val()) {
				showError(_('register_error_password_mismatch'), $('#line-password-confirm'), false);
			}
			if(!txtPassword.val()) {
				showError(_('register_error_password_empty'), $('#line-password'), false);
			} else if(!checkPasswordValid(txtUsername.val(), txtPassword.val())) {
				showError(_('register_error_password_invalid'), $('#line-password'), false);
			} else {
				if (mode=='mobile'){
					clearMessage('#line-password');
					$('#span-toggle .icon-right').remove();
					$('<span/>').addClass('icon-right fr').prependTo($('#span-toggle'));
				} else {
					clearMessage('#line-password');
					$('<span/>').addClass('icon-right').appendTo($('#line-password'));
				}
				return 0;
			}
		}
		function validatePasswordConfirm() {
			if (!txtPasswordConfirm.val()) {

			} else if (txtPassword.val() != txtPasswordConfirm.val()) {
				showError(_('register_error_password_mismatch'), $('#line-password-confirm'), false);
			} else {
				clearMessage('#line-password-confirm');
				$('<span/>').addClass('icon-right').appendTo($('#line-password-confirm'));
				return 0;
			}
		}
		function validateMobile(is_submit){
			if (!txtMobile.val()) {
				showError(_('register_error_mobile_number_empty'), $('#line-mobile-number'), false);
			} else {
				if(is_submit===true){return 0;}
				requestJson(SSO_URL_API_REG_CHECK, {'mobile_no' : spanCountryCode.text()+txtMobile.val()}, function(data) {
					if ('error' in data) {
						if (data['error'] == 'error_mobile_bind_limit') {
							showError(_('register_error_mobile_bind_limit'), $('#line-mobile-number'), false);
						} else if (data['error'] == 'error_mobile_no') {
							showError(_('register_error_mobile_no'), $('#line-mobile-number'), false);
						} else if (data['error'] == 'error_otp_send_limit') {
							showError(_('register_error_otp_send_limit'), $('#line-mobile-number'), false);
						}
					} else {
						clearMessage('#line-mobile-number');
						$('<span/>').addClass('icon-right').appendTo($('#line-mobile-number'));
					}
				})
			}
		}

		txtPassword.blur(validatePassword);
		txtEmail.blur(validateEmail);
		txtUsername.blur(validateUsername);

		if (mode=='pc') {
			txtPasswordConfirm.blur(validatePasswordConfirm);
			checkbox.click(function(){
				if (checkbox.hasClass('checked')){
					checkbox.removeClass('checked');
				} else {
					checkbox.addClass('checked');
					clearMessage('#line-terms');
				}
			});
		}
		// else {
		// 	txtMobile.blur(validateMobile)
		// }

		/* confirm */
		btnRegister.click(function(){
			if(validateEmail(true)!=0){return;}
			var email = txtEmail.val();
			if(validateUsername(true)!=0){return;}
			var username = txtUsername.val();
			if(validatePassword()!=0){return;}
			var password = txtPassword.val();
			if (mode=='pc' && validatePasswordConfirm()!=0){return;}
			var request = {
				'username' : username,
				'email' : email,
				'password' : RSA(password),
				'location' : selectCountry.val(),
				'redirect_uri': getRequestParam('redirect_uri')
			};
			if($('.sso_captcha').is(":visible")) {
				var captcha = rowCaptcha1.find('input').val();
				if(captcha == null || captcha.length <= 0) {
					showError(_('register_error_captcha_empty'), $('#line-captcha'), false);
					refreshCaptcha();
					return;
				}
				request['captcha_key'] = captcha_key;
				request['captcha'] = captcha;
			}
			if(mode=='pc' && !checkbox.hasClass('checked')){
				showError(_('register_error_agree_terms'), $('#line-terms'), false);
				return;
			}
			btnRegister.attr("disabled", true);
			requestJson(SSO_URL_API_REG, request, function(data) {
				if('error' in data) {
					btnRegister.attr("disabled", false);
					if(data['error'] == 'error_require_captcha') {
						showCaptcha();
						return;
					} else if(data['error'] == 'error_captcha') {
						showError(_('register_error_captcha'), $('#line-captcha'), false);
					} else if(data['error'] == 'error_username_existed') {
						showError(_('register_error_username_existed'), $('#line-username'), false);
					} else if(data['error'] == 'error_email_existed') {
						showError(_('register_error_email_existed'), $('#line-email'), false);
					} else {
						showError(_('register_' + data.error), $('#line-btn'), false);
					}
					refreshCaptcha();
				} else {
					showRegisterFinishPage(mode);
				}
			}, 'POST');
			// } else {
			// 	if(validateMobile(true)!=0){return;}
			// 	mobile_register_request['username'] = username;
			// 	mobile_register_request['email'] = email;
			// 	mobile_register_request['mobile_no1'] = spanCountryCode.text();
			// 	mobile_register_request['mobile_no2'] = txtMobile.val();
			// 	mobile_register_request['password'] = password;
			// 	mobile_register_request['country'] = selectCountry.val();
			// 	request = {
			// 		'username' : username,
			// 		'email' : email,
			// 		'mobile_no' : spanCountryCode.text()+txtMobile.val()
			// 	};
			// 	requestJson(SSO_URL_API_REG_CHECK, request, function(data) {
			// 		if ('error' in data) {
			// 			if (data['error'] == 'error_mobile_bind_limit') {
			// 				showError(_('register_error_mobile_bind_limit'), $('#line-mobile-number'), false);
			// 			} else if (data['error'] == 'error_mobile_no') {
			// 				showError(_('register_error_mobile_no'), $('#line-mobile-number'), false);
			// 			} else if (data['error'] == 'error_otp_send_limit') {
			// 				showError(_('register_error_otp_send_limit'), $('#line-mobile-number'), false);
			// 			} else if (data['error'] == 'error_username_existed') {
			// 				showError(_('register_error_username_existed'), $('#line-username'), false);
			// 			} else if (data['error'] == 'error_email_existed') {
			// 				showError(_('register_error_email_existed'), $('#line-email'), false);
			// 			}
			// 		} else {
			// 			showRegisterConfirmMobilePage();
			// 		}
			// 	})
			// }
		});
	}

	function showRegisterConfirmMobilePage() {
		var panelRegisterConfirmMobile = $('<div/>');
		$('<h2/>').addClass('title').text(_('register_confirm_mobile_header')).appendTo(panelRegisterConfirmMobile);
		var issues = $('<div/>', {'id': 'line-issues', 'class': 'issues center'}).appendTo(panelRegisterConfirmMobile);

		$('<p/>').html(_('register_confirm_mobile_description')).appendTo(issues);
		$('<p/>').addClass('emphasis').text(mobile_register_request['mobile_no1']+' '+mobile_register_request['mobile_no2']).appendTo(issues);

		var lineSendCode = $('<div/>', {'id': 'line-send-code'}).appendTo(panelRegisterConfirmMobile);
		var btnSendCode = $('<a/>', {'href': 'javascript:;', 'class': 'btn btn-login'}).text(_('send_confirmation_code')).appendTo(lineSendCode);
		var btnEditMobile = $('<a/>', {'href': 'javascript:;', 'class': 'btn btn-grey'}).text(_('edit_mobile_no')).appendTo(panelRegisterConfirmMobile);

		showContent(panelRegisterConfirmMobile, 'mobile');

		btnEditMobile.click(function(){
			showRegisterPage('mobile');
		});

		btnSendCode.click(function(){
			var request = {
				'username' : mobile_register_request['username'],
				'email' : mobile_register_request['email'],
				'mobile_no' : mobile_register_request['mobile_no1'] + mobile_register_request['mobile_no2']
			};
			requestJson(SSO_URL_API_REG_PREPARE, request, function(data) {
				if ('error' in data) {
					if (data['error'] == 'error_require_captcha') {
						showRegisterCaptchaPage();
					} else {
						showMobileRegisterAlert(_('register_' + data.error));
					}
				} else {
					showRegisterVerifyMobilePage();
				}
			})
		})
	}

	function showRegisterCaptchaPage() {
		var panelRegisterCaptcha = $('<div/>');
		$('<h2/>').addClass('title').text(_('register_captcha_header')).appendTo(panelRegisterCaptcha);

		var formRegisterCaptcha = $('<form/>', {'id': 'form-register'}).addClass('loginForm').appendTo(panelRegisterCaptcha).submit(false);
		var captchaWrapper = $('<div/>', {'class': 'line clearfix', 'id': 'sso_captcha_wrap'}).appendTo(formRegisterCaptcha);
		var rowCaptcha1 = $('<div/>', {'id': 'line-captcha', 'class': 'sso_captcha'}).appendTo(captchaWrapper);
		var inputCaptcha = $('<input/>', {
			'id': 'input-captcha', 'name': 'captcha', 'type': 'tel', 'placeholder': _('register_form_captcha_prompt'), 'autocorrect': 'off', 'autocapitalize': 'off'
		}).appendTo(rowCaptcha1);
		var rowCaptcha2 = $('<div/>', {'id': 'sso_captcha_image', 'class': 'clearfix sso_captcha'}).appendTo(captchaWrapper);
		captcha_key = uuid().replace(/-/g,'');
		$('<img/>', {'src': CAPTCHA_SERVICE+'?key='+captcha_key}).appendTo($('<span/>', {'class': 'code fl'}).appendTo(rowCaptcha2));
		var refresh_captcha = $('<a/>', {'href': 'javascript:;', 'class': 'refresh fl'}).text(_('refresh_captcha')).appendTo(rowCaptcha2);
		refresh_captcha.click(refreshCaptcha);
		refreshCaptcha();

		var lineVerify = $('<div/>', {'id': 'line-verify'}).appendTo(panelRegisterCaptcha);
		var btnVerify = $('<a/>', {'href': 'javascript:;', 'class': 'btn btn-login'}).text(_('btn_captcha_verify')).appendTo(lineVerify);

		showContent(panelRegisterCaptcha, 'mobile');

		btnVerify.click(function(){
			var request = {
				'username' : mobile_register_request['username'],
				'email' : mobile_register_request['email'],
				'mobile_no' : mobile_register_request['mobile_no1'] + mobile_register_request['mobile_no2']
			};
			var captcha = rowCaptcha1.find('input').val();
			if(captcha == null || captcha.length <= 0) {
				showError(_('register_error_captcha_empty'), $('#line-captcha'));
				refreshCaptcha();
				return;
			}
			request['captcha_key'] = captcha_key;
			request['captcha'] = captcha;
			requestJson(SSO_URL_API_REG_PREPARE, request, function(data) {
				if ('error' in data) {
					if (data['error'] == 'error_captcha') {
						showError(_('register_error_captcha'), $('#line-captcha'));
						inputCaptcha.val('');
						refreshCaptcha();
					} else {
						showMobileRegisterAlert(_('register_' + data.error));
					}
				} else {
					showRegisterVerifyMobilePage();
				}
			})
		});

		function onKeyPress(e) {
			if(e.which == KEY_CODE_ENTER) {
				e.preventDefault();
				if(!inputCaptcha.val()) {
					inputCaptcha.focus();
					return;
				}
				btnVerify.trigger('click');
			}
		}
		$('.content').keypress(onKeyPress);
	}

	function showRegisterVerifyMobilePage() {
		var panelRegisterVerifyMobile = $('<div/>');
		$('<h2/>').addClass('title').text(_('register_verify_mobile_header')).appendTo(panelRegisterVerifyMobile);
		var issues = $('<div/>', {'id': 'line-issues', 'class': 'issues center'}).appendTo(panelRegisterVerifyMobile);

		$('<p/>').html(_('register_verify_mobile_description')).appendTo(issues);
		$('<p/>').addClass('emphasis').text(mobile_register_request['mobile_no1']+' '+mobile_register_request['mobile_no2']).appendTo(issues);

		var resendLine = $('<div/>', {'id': 'line-resend', 'class': 'resendLine clearfix'}).appendTo(panelRegisterVerifyMobile);
		var verification_code = $('<input/>', {
			'type': 'tel', 'placeholder': _('register_verify_mobile_prompt'), 'class': 'fl'
		}).appendTo(resendLine);
		var btnResend = $('<a/>', {'href': 'javascript:;', 'class': 'btn btn-grey btn-resend fr'}).text(_('btn_register_verify_resend')).appendTo(resendLine);

		var lineVerify = $('<div/>', {'id': 'line-verify'}).appendTo(panelRegisterVerifyMobile);
		var btnVerify = $('<a/>', {'href': 'javascript:;', 'class': 'btn btn-login'}).text(_('btn_register_verify')).appendTo(lineVerify);

		showContent(panelRegisterVerifyMobile, 'mobile');
		btnResend.prop("disabled", true);
		resendCooldown();

		function resendCooldown(){
			var leftSeconds = OTP_REGISTER_INTERVAL;
			btnResend.text(_('btn_register_verify_resend_otp_timer', {'second': leftSeconds}));
			btnResend.addClass('cooldown');
			var timer = setInterval(function () {
				leftSeconds--;
				if(leftSeconds <= 0) {
					btnResend.text(_('btn_register_verify_resend')).prop("disabled", false);
					btnResend.removeClass('cooldown');
					clearInterval(timer);
				} else {
					btnResend.text(_('btn_register_verify_resend_otp_timer', {'second': leftSeconds}));
				}
			}, 1000);
		}

		btnResend.click(function(){
			if (btnResend.prop("disabled")) {return;}
			btnResend.prop("disabled", true);
			var request = {
				'username' : mobile_register_request['username'],
				'email' : mobile_register_request['email'],
				'mobile_no' : mobile_register_request['mobile_no1'] + mobile_register_request['mobile_no2']
			};
			requestJson(SSO_URL_API_REG_PREPARE, request, function(data) {
				if ('error' in data) {
					if (data['error'] == 'error_require_captcha') {
						showRegisterCaptchaPage();
					} else {
						showMobileRegisterAlert(_('register_' + data.error));
					}
				} else {
					resendCooldown();
				}
			})
		});

		btnVerify.click(function(){
			if(!verification_code.val()){
				showError(_('register_error_captcha_empty'), $('#line-resend'));
				return;
			}
			if (btnVerify.prop("disabled")) {return;}
			btnVerify.prop("disabled", true);
			var request = {
				'username' : mobile_register_request['username'],
				'email' : mobile_register_request['email'],
				'password': RSA(mobile_register_request['password']),
				'mobile_no' : mobile_register_request['mobile_no1'] + mobile_register_request['mobile_no2'],
				'location': mobile_register_request['country'],
				'redirect_uri': getRequestParam('redirect_uri'),
				'otp': verification_code.val(),
				'locale': getRequestParam('locale')
			};
			requestJson(SSO_URL_API_REG, request, function(data) {
				if('error' in data) {
					if (data['error'] == 'error_otp') {
						showError(_('register_error_otp'), $('#line-resend'));
					} else {
						showMobileRegisterAlert(_('register_' + data.error));
						return;
					}
					btnVerify.prop("disabled", false);
				} else {
					showRegisterFinishPage('mobile');
				}
			}, 'POST');
		});
	}

	function showRegisterFinishPage(mode){
		var panelRegisterFinish = $('<div/>');
		$('<h2/>').addClass('title').text(_('register_finish_header')).appendTo(panelRegisterFinish);
		var issues = $('<div/>').addClass('issues').appendTo(panelRegisterFinish);

		if (mode=='pc'){
			$('<p/>').text(_('register_finish_description')).appendTo(issues);
			var ol = $('<ol/>').appendTo(issues);
			var verifyEmail = $('<li/>').addClass('issueItem').text(_('register_finish_verify_email_pc_part1')).appendTo(ol).append($('<br/>'));
			$('<span/>', {'style':'color: red;'}).text(_('register_finish_verify_email_pc_part2')).appendTo(verifyEmail);
			verifyEmail.append($('<br/>'));
			$('<a/>', {
				'href': SETTINGS.test?ACCOUNT_CENTER_TEST_URL:ACCOUNT_CENTER_URL, 'target': '_blank', 'class': 'newlineLink'
			}).text(_('register_finish_verify_email_link')).appendTo(verifyEmail);

			var country = SETTINGS.country;
			if (country == 'TW') {
				var verifyUser = $('<li/>').text(_('register_finish_verify_user')).appendTo(ol).append($('<br>'));
				$('<a/>', {
					'href': _('link_verify_user'), 'target': '_blank', 'class': 'newlineLink'
				}).text(_('register_finish_verify_user_link')).appendTo(verifyUser);
			}
			var bindMobile = $('<li/>').text(_('register_finish_bind_mobile')).appendTo(ol).append($('<br>'));
			$('<a/>', {
				'href': _('link_bind_mobile'), 'target': '_blank', 'class': 'newlineLink'
			}).text(_('register_finish_bind_mobile_link')).appendTo(bindMobile);
		} else {
			$('<p/>').addClass('issueItem').text(_('register_finish_verify_email_mobile_part1')).appendTo(issues);
			$('<p/>').addClass('issueItem').html(_('register_finish_verify_email_mobile_part2',
					{'tag_begin':'<a id="link-verify-email" href="javascript:;">','tag_end':'</a>'})
			).appendTo(issues);
		}

		var btn_back = $('<a/>', {'id': 'confirm-btn', 'href':'javascript:;', 'class': 'btn btn-login'}).text(_('btn_register_finish')).appendTo(panelRegisterFinish);
		btn_back.click(function(){
			var redirect_uri = getRequestParam('redirect_uri');
			if (redirect_uri == null) {
				redirect_uri = DEFAULT_REDIRECT_URL
			}
			redirect(redirect_uri);
		});

		showContent(panelRegisterFinish, mode, true);
	}

	function showGrantPage(mode) {
		var panelAuth = $('<div/>');
		var wrapper = $('<div/>').addClass('linkWrapper').appendTo(panelAuth);
		var icon_group = $('<div/>').addClass('linkImg').appendTo(wrapper);
		$('<img/>', {'src': SETTINGS['app_icon']}).appendTo(icon_group);
		//var user_icon = $('<div/>').addClass('user-icon icon-item').appendTo(icon_group);
		//var guser_i = $('<i/>').addClass('img').appendTo(user_icon);

		var description = $('<div/>').addClass('linkTips').text(_('grant_form_caption')).appendTo(icon_group);
		var app_name = $('<span/>').text(SETTINGS['app_name']).addClass('game-name');
		description.prepend(app_name);

		var btn_group = $('<div/>').addClass('linkBtn').appendTo(wrapper);
		var btnDecline = $('<a/>', {
			'id': 'sso_auth_from_decl_button', 'type': 'submit', 'text': _('grant_form_button_decline')
		}).addClass('btn-cancel').appendTo(btn_group);
		var btnAuthorize = $('<a/>', {
			'id': 'sso_auth_from_auth_button', 'type': 'submit', 'text': _('grant_form_button_confirm')
		}).addClass('btn-confirm').appendTo(btn_group);

		var panelLink = $('<div/>').addClass('linkChange').appendTo(panelAuth);
		var linkChangeAccount = $('<a/>', {
			'id':'auth_link_change_account', 'text': _('grant_link_change_account_text'), 'href': 'javascript:;'
		}).appendTo(panelLink);
		showContent(panelAuth, mode);

		btnAuthorize.click(function auth(){
			var params = getRequestParams();
			params['grant'] = 1;
			requestJson(SSO_URL_API_AUTH, params, SSO_SERVER.grantTokenCallback, 'POST');
		});
		btnDecline.click(function decline(){
			redirect(getRequestParam('redirect_uri'), {'error':'access_denied'});
		});
		linkChangeAccount.click(function changeAccount(){
			removeCookie(SETTINGS.session_cookie_name, SETTINGS.session_cookie_domain);
			showLoginPage(mode);
		});
	}

	//function showLoginPage(mode) {
	//	var panelLogin = $('<div/>');
	//	appendPlatformsPanel(panelLogin);

	//	$('<h2/>').addClass('title').text(_('login_header')).prependTo(panelLogin);

	//	var formLogin = $('<form/>', {'id': 'login-form'}).addClass('loginForm').appendTo(panelLogin);

	//	var mobileAccount = false;

	//	var lineCountry = $('<div/>', {'id': 'line-country', 'style': 'display: none;'}).addClass('line').appendTo(formLogin);
	//	var selectCountry = $('<select/>', {
	//		'id': 'sso_login_form_select_country', 'name': 'select_country'
	//	}).addClass('country').appendTo(lineCountry);
	//	for (i in COUNTRY_LIST) {
	//		var option = $('<option/>', {'value': COUNTRY_LIST[i].code, 'data-country-code':  COUNTRY_LIST[i].pcode});
	//		var content = COUNTRY_LIST[i].name + " ";
	//		if(COUNTRY_LIST[i].hasOwnProperty("lname")){
	//			content = content + "(" + COUNTRY_LIST[i].lname + "\u200e) ";
	//		}
	//		$('<p/>').text(content).appendTo(option);
	//		selectCountry.append(option);
	//	}

	//	var lineAccount = $('<div/>', {'id': 'line-account'}).addClass('line').appendTo(formLogin);
	//	var txtCountryCode = $('<span/>', {
	//		'id': 'sso_login_form_country_code', 'style': 'display: none;'
	//	}).addClass('country-code').appendTo(lineAccount);
	//	selectCountry.change(function () {
	//		var selected = $(this).find(":selected");
	//		txtCountryCode.text('+' + selected.data('country-code'));
	//	});
	//	selectCountry.val(SETTINGS.country).change();
	//	var txtAccount = $('<input/>', {
	//		'id': 'sso_login_form_account', 'name': 'username', 'type': 'text', 'placeholder': _('login_form_account_prompt'), 'autocorrect': 'off', 'autocapitalize': 'off'
	//	}).appendTo(lineAccount);
	//	if (SETTINGS.test && SETTINGS.language=='vi' && /android/i.test(navigator.userAgent)){
	//		txtAccount.attr('placeholder', _('login_form_account_prompt_vi'));
	//	}
	//	var txtPassword = $('<input/>', {
	//		'id': 'sso_login_form_password', 'name': 'password', 'type': 'password', 'placeholder': _('login_form_password_prompt')
	//	}).appendTo($('<div/>', {'id': 'line-password'}).addClass('line').appendTo(formLogin));

	//	var captchaWrapper = $('<div/>', {'class': 'line clearfix', 'id': 'sso_captcha_wrap'}).appendTo(formLogin);
	//	var rowCaptcha1 = $('<div/>', {'id': 'line-captcha', 'class': 'sso_captcha'}).appendTo(captchaWrapper);
	//	$('<input/>', {
	//		'id': 'input-captcha', 'name': 'captcha', 'type': 'tel', 'placeholder': _('login_form_captcha_prompt'), 'autocorrect': 'off', 'autocapitalize': 'off'
	//	}).appendTo(rowCaptcha1);
	//	var rowCaptcha2 = $('<div/>', {'id': 'sso_captcha_image', 'class': 'clearfix sso_captcha'}).appendTo(captchaWrapper);
	//	$('<img/>').appendTo($('<span/>', {'class': 'code fl'}).appendTo(rowCaptcha2));
	//	var refresh_captcha = $('<a/>', {'href': 'javascript:;', 'class': 'refresh fl'}).text(_('refresh_captcha')).appendTo(rowCaptcha2);
	//	refresh_captcha.click(refreshCaptcha);
	//	rowCaptcha1.hide();
	//	rowCaptcha2.hide();

	//	var btnLogin = $('<input/>', {
	//		'id': 'confirm-btn', 'name': 'login', 'type': 'submit', 'value': _('login_form_button_login'), 'onclick': 'javascript:return false;'
	//	}).addClass('btn').appendTo($('<div/>', {'id': 'line-btn', 'class': 'line btnLine'}).appendTo(formLogin));

	//	var dividerOr = $('<span/>').text(_('sso_login_divider_text')).appendTo($('<div/>', {'class': 'divider'}).appendTo(formLogin));

	//	var btnRegister = $('<input/>', {
	//		'id': 'sso_login_link_register', 'name': 'register', 'type': 'button', 'value': _('sso_login_link_register_text'), 'onclick': 'javascript:return false;'
	//	}).addClass('btn').addClass('black').appendTo($('<div/>', {'id': 'line-btn', 'class': 'line btnLine'}).appendTo(formLogin));

	//	if (SETTINGS.fb_platform_login) {
	//		var fbPlatformLogin = $('<button/>', {'id': 'fb-platform-login-btn', 'name': 'fb-platform-login', 'type': 'submit',
	//			'text': _('fb_platform_login'), 'onclick': 'javascript:return false;'
	//		}).addClass('btn fb').appendTo($('<div/>', {'id': 'fb-platform-login-btn-line', 'class': 'line btnLine'}).appendTo(formLogin));
	//		fbPlatformLogin.click(initLoginFacebookPlatform);
	//	}

	//	var panelLink = $('<div/>').addClass('linkLine').appendTo(panelLogin);

	//	// $('<span/>', {'role': 'separator', 'aria-hidden': 'true'}).text(_('sso_login_link_separator')).appendTo(panelLink);
	//	var linkForgetPassword = $('<a/>', {
	//		'id':'sso_login_link_forget_password', 'text': _('sso_login_link_forget_password_text'), 'href': 'javascript:;'
	//	}).addClass('sec').appendTo(panelLink);

	//	var username = getRequestParam('username');
	//	if(username != null) {
	//		txtAccount.val(username);
	//		txtPassword.focus();
	//	}

	//	showContent(panelLogin, mode);

	//	btnRegister.click(function register() {
	//		var requestParams = getRequestParams();
	//		if (inIframe) {
	//			var register_url = SSO_URL_UI_REGISTER;
	//			if('locale' in requestParams) {
	//				register_url += '?locale=' + requestParams['locale'];
	//			}
	//			window.open(register_url, '_blank');
	//		} else {
	//			var params = {'redirect_uri': window.location.href};
	//			if('display' in requestParams) {
	//				params['display'] = requestParams['display'];
	//			}
	//			if('locale' in requestParams) {
	//				params['locale'] = requestParams['locale'];
	//			}
	//			redirect(SSO_URL_UI_REGISTER, params);
	//		}
	//	});

	//	linkForgetPassword.click(function forgetPassword() {
	//		if (SETTINGS.test){
	//			window.open(ACCOUNT_CENTER_RECOVERY_TEST_URL, '_blank');
	//		} else {
	//			window.open(ACCOUNT_CENTER_RECOVERY_URL, '_blank');
	//		}
	//	});

	//	txtAccount.blur(checkMobile);
	//	function checkMobile() {
	//		var account = txtAccount.val();
	//		if (/^\d{6,15}$/.test(account)) {
	//			mobileAccount = true;
	//			lineCountry.show();
	//			txtCountryCode.show();
	//			txtAccount.addClass('mobile-number');
	//		} else {
	//			mobileAccount = false;
	//			lineCountry.hide();
	//			txtCountryCode.hide();
	//			txtAccount.removeClass('mobile-number');
	//		}
	//	}

	//	function login() {
	//		checkMobile();
	//		var account;
	//		if (mobileAccount) {
	//			account = txtCountryCode.text() + txtAccount.val();
	//		} else {
	//			account = txtAccount.val();
	//		}
	//		var password = txtPassword.val();
	//		if(account.length <= 0) {
	//			showError(_('login_error_account_empty'), $('#line-account'));
	//			txtAccount.focus();
	//			return;
	//		}
	//		if(password.length <= 0) {
	//			showError(_('login_error_password_empty'), $('#line-password'));
	//			txtPassword.focus();
	//			return;
	//		}
	//		var request = {'account': account};
	//		if($('.sso_captcha').is(":visible")) {
	//			var captcha = rowCaptcha1.find('input').val();
	//			if(captcha == null || captcha.length <= 0) {
	//				showError(_('login_error_captcha_empty'), $('#sso_captcha_wrap'));
	//				refreshCaptcha();
	//				return;
	//			}
	//			request['captcha_key'] = captcha_key;
	//			request['captcha'] = captcha;
	//		}
	//		btnLogin.attr("disabled", true);
	//		requestJson(SSO_URL_API_PRELOGIN, request, SSO_SERVER.preloginCallback);
	//	}

		function onKeyPress(e) {
			if(e.which == KEY_CODE_ENTER) {
				e.preventDefault();
				if(btnLogin.attr("disabled")) {
					return;
				}
				if(txtAccount.val().length <= 0) {
					if(txtAccount.is(':focus')) {
						showError(_('login_error_account_empty'), $('#line-account'));
					}
					txtAccount.focus();
					return;
				}
				if(txtPassword.val().length <= 0) {
					if(txtPassword.is(':focus')) {
						showError(_('login_error_password_empty'), $('#line-password'));
					}
					txtPassword.focus();
					return;
				}
				login();
			}
		}

		$('.content').keypress(onKeyPress);
		btnLogin.click(login);

		txtAccount.focus();

		if (SETTINGS.iframe) {
			iframeDetectHeight();
		}
	}

	//function showLoginPageBeetalk(mode) {
	//	var panelLogin = $('<div/>');
	//	appendPlatformsPanel(panelLogin);

	//	$('<h2/>').addClass('title').text(_('login_header')).prependTo(panelLogin);

	//	var formLogin = $('<form/>', {'id': 'login-form'}).addClass('loginForm').appendTo(panelLogin);

	//	var selectCountry = $('<select/>', {
	//		'id': 'sso_register_form_select_country', 'name': 'select_country'
	//	}).appendTo($('<div/>', {'id': 'line-select-country'}).addClass('line').appendTo(formLogin));
	//	for (i in COUNTRY_LIST){
	//		var option = $('<option/>', {'value': COUNTRY_LIST[i].code});
	//		var content = COUNTRY_LIST[i].name + " ";
	//		if(COUNTRY_LIST[i].hasOwnProperty("lname")){
	//			content = content + "(" + COUNTRY_LIST[i].lname + "\u200e) ";
	//		}
	//		$('<p/>').text(content).appendTo(option);
	//		selectCountry.append(option);
	//	}
	//	var mobileLine = $('<div/>', {'id': 'line-mobile-number', 'class': 'line phoneLine'}).appendTo(formLogin);
	//	var spanCountryCode = $('<span/>', {'id': 'country-code', 'class': 'countryCode'}).appendTo(mobileLine);
	//	var txtMobile = $('<input/>', {
	//		'id': 'sso_register_form_mobile_number', 'type': 'tel', 'placeholder': _('register_form_mobile_number_prompt')
	//	}).appendTo(mobileLine).val(mobile_register_request['mobile_no2']);
	//	selectCountry.change(function(){
	//		for (i in COUNTRY_LIST){
	//			if (COUNTRY_LIST[i]['code']==this.value){
	//				spanCountryCode.text('+'+COUNTRY_LIST[i]['pcode']);
	//				break;
	//			}
	//		}
	//	});
	//	selectCountry.val(SETTINGS.country).change();

	//	var resendLine = $('<div/>', {'id': 'line-send-code', 'class': 'resendLine clearfix'}).appendTo(formLogin).hide();
	//	var txtCode = $('<input/>', {
	//		'type': 'tel', 'placeholder': _('login_form_verification_prompt'), 'class': 'fl'
	//	}).appendTo(resendLine);
	//	var btnSend = $('<a/>', {'id': 'send-btn', 'href': 'javascript:;', 'class': 'btn btn-grey btn-resend fr'}).text(_('login_form_button_send')).appendTo(resendLine);

	//	var linePassword = $('<div/>', {'id': 'line-password'}).addClass('line').appendTo(formLogin);
	//	var txtPassword = $('<input/>', {
	//		'id': 'sso_login_form_password', 'name': 'password', 'type': 'password', 'placeholder': _('login_form_password_prompt')
	//	}).appendTo(linePassword);

	//	var loginMethod = $('<p/>').addClass('text-right').appendTo(formLogin);
	//	var loginTxt = $('<a/>', {'href': 'javascript:;'}).text(_('login_platform_beetalk_otp_prompt')).appendTo(loginMethod);

	//	var captchaWrapper = $('<div/>', {'class': 'line clearfix', 'id': 'sso_captcha_wrap'}).appendTo(formLogin);
	//	var rowCaptcha1 = $('<div/>', {'id': 'line-captcha', 'class': 'sso_captcha'}).appendTo(captchaWrapper);
	//	$('<input/>', {
	//		'id': 'input-captcha', 'name': 'captcha', 'type': 'tel', 'placeholder': _('login_form_captcha_prompt')
	//	}).appendTo(rowCaptcha1);
	//	var rowCaptcha2 = $('<div/>', {'id': 'sso_captcha_image', 'class': 'clearfix sso_captcha'}).appendTo(captchaWrapper);
	//	$('<img/>').appendTo($('<span/>', {'class': 'code fl'}).appendTo(rowCaptcha2));
	//	var refresh_captcha = $('<a/>', {'href': 'javascript:;', 'class': 'refresh fl'}).text(_('refresh_captcha')).appendTo(rowCaptcha2);
	//	refresh_captcha.click(refreshCaptcha);
	//	rowCaptcha1.hide();
	//	rowCaptcha2.hide();

	//	var btnLogin = $('<input/>', {
	//		'id': 'confirm-btn', 'name': 'login', 'type': 'submit', 'value': _('login_form_button_login'), 'onclick': 'javascript:return false;'
	//	}).addClass('btn').appendTo($('<div/>', {'id': 'line-btn', 'class': 'line btnLine'}).appendTo(formLogin));

	//	showContent(panelLogin, mode);

	//	var otpLoginState = false;
	//	var isShowCaptcha = false;
	//	loginMethod.click(function() {
	//		if (otpLoginState){
	//			resendLine.hide();
	//			linePassword.show();
	//			loginTxt.text(_('login_platform_beetalk_otp_prompt'));
	//			if (isShowCaptcha){
	//				showCaptcha();
	//			}
	//		} else {
	//			linePassword.hide();
	//			resendLine.show();
	//			loginTxt.text(_('login_platform_beetalk_pw_prompt'));
	//			var captcha = $('.sso_captcha');
	//			if(captcha.is(":visible")) {
	//				captcha.hide();
	//				isShowCaptcha = true;
	//			}
	//		}
	//		otpLoginState = !otpLoginState;
	//	});

	//	function sendOtp() {
	//		if ($('#send-btn').attr("disabled")){
	//			return true;
	//		}
	//		var countryCode = spanCountryCode.text();
	//		var account = txtMobile.val();
	//		if(account.length <= 0) {
	//			showError(_('beetalk_login_error_account_empty'), $('#line-mobile-number'));
	//			txtMobile.focus();
	//			return;
	//		}
	//		if(!/^\d{6,15}$/.test(account)){
	//			showError(_('beetalk_login_error_account_invalid'), $('#line-mobile-number'));
	//			txtMobile.focus();
	//			return;
	//		}
	//		var request = {'account': countryCode + account};
	//		var locale = getRequestParam('locale');
	//		if(locale) {
	//			request['locale'] = locale;
	//		}
	//		requestJson(SSO_URL_API_BEETALK_SEND_OTP, request, SSO_SERVER.beetalkSendOtpCallback);
	//	}

	//	function login() {
	//		var countryCode = spanCountryCode.text();
	//		var account = txtMobile.val();
	//		if(account.length <= 0) {
	//			showError(_('beetalk_login_error_account_empty'), $('#line-mobile-number'));
	//			txtMobile.focus();
	//			return;
	//		}
	//		if(!/^\d{6,15}$/.test(account)){
	//			showError(_('beetalk_login_error_account_invalid'), $('#line-mobile-number'));
	//			txtMobile.focus();
	//			return;
	//		}
	//		if(otpLoginState) {
	//			var otpCode = txtCode.val();
	//			if(otpCode.length <= 0) {
	//				showError(_('beetalk_otp_login_error_password_empty'), $('#line-send-code'));
	//				txtCode.focus();
	//				return;
	//			}
	//			var request = {
	//				'account' : countryCode + account,
	//				'otp' : otpCode
	//			};
	//			var redirect_uri = getRequestParam('redirect_uri');
	//			if(redirect_uri) {
	//				request['redirect_uri'] = redirect_uri;
	//			}
	//			btnLogin.attr("disabled", true);
	//			requestJson(SSO_URL_API_BEETALK_LOGIN_BY_OTP, request, SSO_SERVER.beetalkOtpLoginCallback);
	//		} else {
	//			var password = txtPassword.val();
	//			if(password.length <= 0) {
	//				showError(_('login_error_password_empty'), $('#line-password'));
	//				txtPassword.focus();
	//				return;
	//			}
	//			var request = {'account': countryCode + account};
	//			if($('.sso_captcha').is(":visible")) {
	//				var captcha = rowCaptcha1.find('input').val();
	//				if(captcha == null || captcha.length <= 0) {
	//					showError(_('login_error_captcha_empty'), $('#line-captcha'));
	//					refreshCaptcha();
	//					return;
	//				}
	//				request['captcha_key'] = captcha_key;
	//				request['captcha'] = captcha;
	//			}
	//			btnLogin.attr("disabled", true);
	//			requestJson(SSO_URL_API_BEETALK_PRELOGIN, request, SSO_SERVER.beetalkPreloginCallback);
	//		}
	//	}

	//	function onKeyPress(e) {
	//		if(e.which == KEY_CODE_ENTER) {
	//			if(btnLogin.attr("disabled")) {
	//				return;
	//			}
	//			if(txtMobile.val().length <= 0) {
	//				if(txtMobile.is(':focus')) {
	//					showError(_('beetalk_login_error_account_empty'), $('#line-mobile-number'));
	//				}
	//				txtMobile.focus();
	//				return;
	//			}
	//			if(!/^\d{6,15}$/.test(txtMobile.val())){
	//				showError(_('beetalk_login_error_account_invalid'), $('#line-mobile-number'));
	//				txtMobile.focus();
	//				return;
	//			}
	//			if(txtPassword.val().length <= 0) {
	//				if(txtPassword.is(':focus')) {
	//					showError(_('login_error_password_empty'), $('#line-password'));
	//				}
	//				txtPassword.focus();
	//				return;
	//			}
	//			login();
	//		}
	//	}

	//	$('.content').keypress(onKeyPress);
	//	btnSend.click(sendOtp);
	//	btnLogin.click(login);

	//	txtMobile.focus();
	//}

	function updateURLParameter(url, paramName, paramVal){
		var tempArray = url.split("?");
		var resultURL = tempArray[0]; // baseURL
		for (var i = 1; i < tempArray.length; i++) {
			var queryString = tempArray[i];
			var newQueryString = "";
			var separator = "";
			if (queryString) {
				var queryList = queryString.split("&");
				for (var j = 0; j < queryList.length; j++) {
					if (queryList[j].split('=')[0] != paramName) {
						newQueryString += separator + queryList[j];
						separator = "&";
					}
					else {
						if (paramVal != '') {
							newQueryString += separator + paramName + "=" + paramVal;
							separator = "&";
						}
					}
				}
			}
			resultURL = resultURL + "?" + newQueryString;
		}
		return resultURL
	}

	function loginVk() {
		var params = {};
		params.client_id = SETTINGS.vk_client_id;
		params.display = 'page';
		params.redirect_uri = updateURLParameter(window.location.href.split('#')[0], 'platform', PLATFORM_VK);
		params.scope = 'friends,offline';
		params.response_type = 'token';
		params.v = '5.69';
		redirect(VK_OAUTH_URL, params);
	}

	function loginLine() {
		var params = {};
		params.response_type = 'code';
		params.client_id = SETTINGS.line_client_id;
		params.redirect_uri = updateURLParameter(window.location.href.split('#')[0], 'platform', PLATFORM_LINE);
		params.state = SETTINGS.line_state;
		params.scope = 'profile openid';

		redirect(LINE_OAUTH_URL, params);
	}

	function loginGoogle() {
		var client_id = SETTINGS.google_client_id;
		var redirectUrl = window.location.href.split('?')[0] + '/google'
		var params = getRequestParams()
		params.platform = PLATFORM_GOOGLE
		var sParams = '';
		for(var key in params) {
				sParams += '&' + key + '=' + params[key];
		}
		Cookies.set(GOOGLE_COOKIE_NAME, sParams)
		var form = document.createElement('form');
		form.setAttribute('method', 'GET');
		form.setAttribute('action', GOOGLE_OAUTH_URL);
		var params = {
			'client_id': client_id,
			'redirect_uri': redirectUrl,
			'response_type': 'code',
			'scope': 'profile email openid'
		};

		for (var p in params) {
			var input = document.createElement('input');
			input.setAttribute('type', 'hidden');
			input.setAttribute('name', p);
			input.setAttribute('value', params[p]);
			form.appendChild(input);
		}

		document.body.appendChild(form);
		form.submit();
	};

	function loginHuawei() {
		var params = {};
		params.client_id = SETTINGS.huawei_app_id;
		params.redirect_uri = updateURLParameter(window.location.href.split('#')[0], 'platform', PLATFORM_HUAWEI);
		params.response_type = 'code';
		params.scope = 'https://www.huawei.com/auth/account/base.profile';
		redirect(HUAWEI_OAUTH_URL, params);
	}

	function loginFacebook(){
		var params = {};
		params.client_id = SETTINGS.fb_app_id.toString();
		params.redirect_uri = updateURLParameter(window.location.href.split('#')[0], 'platform', PLATFORM_FACEBOOK);
		params.response_type = 'token';
		params.scope = SETTINGS.fb_scope;
		redirect(FACEBOOK_OAUTH_URL, params);
	}

	function exchangeVkToken(access_token){
		var params = {};
		params.vk_access_token = access_token;
		params.client_id = SETTINGS['app_id'];
		requestJson(SSO_URL_OAUTH_TOKEN_VK_EXCHANGE, params, function(data) {
			params = {};
			if('error' in data) {
				params.error = data.error;
			} else if('access_token' in data){
				params.access_token = data.access_token;
			}else{
				params.error = 'error_unknown';
			}
			redirect(getRequestParam('redirect_uri'), params);
		}, 'post');
	}

	function exchangeLineToken(token){
		var params = {};
		params.line_auth_code = token;
		params.client_id = SETTINGS['app_id'];
		var redirect_uri = updateURLParameter(window.location.href.split('#')[0], 'code', '')
		redirect_uri = updateURLParameter(redirect_uri, 'state', '')
		params.redirect_uri = redirect_uri;
		requestJson(SSO_URL_OAUTH_TOKEN_LINE_EXCHANGE, params, function(data) {
			params = {};
			if('error' in data) {
				params.error = data.error;
			} else if('access_token' in data){
				params.access_token = data.access_token;
			}else{
				params.error = 'error_unknown';
			}
			redirect(getRequestParam('redirect_uri'), params);
		}, 'post');
	}

	function exchangeGoogleToken(google_auth_code){
		var params = {};
		params.google_auth_code = google_auth_code;
		params.client_id = SETTINGS['app_id'];
		params.redirect_uri = window.location.href.split('?')[0] + '/google';
		requestJson(SSO_URL_OAUTH_TOKEN_GOOGLE_EXCHANGE, params, function(data) {
			params = {};
			if('error' in data) {
				params.error = data.error;
			} else if('access_token' in data){
				params.access_token = data.access_token;
			}else{
				params.error = 'error_unknown';
			}
			redirect(getRequestParam('redirect_uri'), params);
		}, 'post');
	}

	function exchangeHuaweiToken(auth_code){
		var params = {};
		params.hw_auth_code = auth_code;
		params.client_id = SETTINGS['app_id'];
		params.redirect_uri = updateURLParameter(window.location.href.split('#')[0], 'authorization_code', '')
		requestJson(SSO_URL_OAUTH_TOKEN_HUAWEI_EXCHANGE, params, function(data) {
			params = {};
			if('error' in data) {
				params.error = data.error;
			} else if('access_token' in data){
				params.access_token = data.access_token;
			}else{
				params.error = 'error_unknown';
			}
			redirect(getRequestParam('redirect_uri'), params);
		}, 'post');
	}

	function exchangeFacebookToken(access_token){
		var params = {};
		params.facebook_access_token = access_token;
		params.client_id = SETTINGS['app_id'];
		requestJson(SSO_URL_OAUTH_TOKEN_FACEBOOK_EXCHANGE, params, function(data) {
			params = {};
			if('error' in data) {
				params.error = data.error;
			} else if('access_token' in data){
				params.access_token = data.access_token;
			}else{
				params.error = 'error_unknown';
			}
			redirect(getRequestParam('redirect_uri'), params);
		}, 'post');
	}

	function initLoginFacebookPlatform(){
		var params = {};
		params.client_id = SETTINGS.fb_gas_app_id.toString();
		params.redirect_uri = window.location.href + "&mode=" + FB_PLATFORM_MODE;
		params.response_type = 'token';
		params.scope = "public_profile,email,user_friends";
		redirect(FACEBOOK_OAUTH_URL, params);
	}

	function loginFacebookPlatform(access_token) {
		var params = getRequestParams();
		params.access_token = access_token;
		params.mode = SETTINGS['mode'];

		requestJson(SSO_URL_API_FACEBOOK_PLATFORM_LOGIN, params, function(data) {
			if ('error' in data) {
				// if error occurs when submit fb token to FACEBOOK_PLATFORM_LOGIN api,
				// dlsplay login page and show error
				if (isMobile()) {
					showLoginPage('mobile');
				} else {
					showLoginPage('pc');
				}
				showError(_('fb_platform_login_' + data['error']), $('#fb-platform-login-btn-line'))
			} else {
				// if successfully login with fb token, redirect to callback url with ssokey or token
				if (SETTINGS['mode'] == 'sso_login') {
					if('redirect_uri' in data) {
						redirect(data['redirect_uri']);
					} else {
						redirect(DEFAULT_REDIRECT_URL, {'session_key': data['session_key']});
					}
				} else if (SETTINGS['mode'] == 'oauth_login') {
					var oauth_params = {};
					if (params['response_type'] in data) {
						oauth_params[params['response_type']] = data[params['response_type']];
					}
					redirect(getRequestParam('redirect_uri'), oauth_params);
				}
			}
		}, 'post');
	}


	function checkLoginVk() {
		var accessToken = getRequestFragment("access_token"),
			userId = getRequestFragment("user_id");
		if (accessToken && userId) {
			exchangeVkToken(accessToken);
			return true;
		}
		return false;
	}

	function checkLoginLine() {
		var code = getRequestParam("code"),
			error = getRequestParam("error"),
			error_reason;

		if (code != null) {
			exchangeLineToken(code)
			return true;
		} else if (error != null && SETTINGS.platform == PLATFORM_LINE) {
				error_reason = 'error_' + getRequestParam("error_reason");
				redirect(getRequestParam('redirect_uri'), {error: error_reason});
				return true;
		}
		return false;
	}

	function checkLoginGoogle() {
		var google_auth_code = getRequestParam("google_auth_code");
		if (google_auth_code) {
			exchangeGoogleToken(google_auth_code);
			return true;
		}
		return false;
	}

	function checkLoginHuawei() {
		var authorization_code = getRequestParam("authorization_code");
		if (authorization_code) {
			exchangeHuaweiToken(authorization_code);
			return true;
		}
		return false;
	}

	function checkLoginFacebook(){
		var accessToken = getRequestFragment("access_token"),
			mode = getRequestParam("mode"),
			error = getRequestParam("error"),
			error_reason;
		if (mode == FB_PLATFORM_MODE) {
			if (accessToken != null) {
				loginFacebookPlatform(accessToken);
				return true;
			} else if (error != null) {
				error_reason = getRequestParam("error_reason");
				setInitCallback(function () {
					showError(_('fb_platform_login_' + error_reason), $('#fb-platform-login-btn-line'));
				});
			}
		} else {
			if (accessToken != null) {
				exchangeFacebookToken(accessToken);
				return true;
			}
			if (error != null && SETTINGS.platform == PLATFORM_FACEBOOK) {
				error_reason = 'error_' + getRequestParam("error_reason");
				redirect(getRequestParam('redirect_uri'), {error: error_reason});
				return true;
			}
		}
		return false;
	}

	function iframeTryPostAfterLogin(data) {
		if (SETTINGS.iframe) {
			parent.postMessage(JSON.stringify(data), SETTINGS.target_origin);
			return true;
		}
		return false;
	}

	function iframeDetectHeight() {
		onElementHeightChange(document.body, function (height) {
			var data = {height: height};
			parent.postMessage(JSON.stringify(data), SETTINGS.target_origin);
		})
	}

	function onElementHeightChange(elm, callback){
		var lastHeight = elm.clientHeight, newHeight;
		callback(lastHeight);
		(function run(){
			newHeight = elm.clientHeight;
			if( lastHeight != newHeight )
				callback(newHeight);
			lastHeight = newHeight;

			if( elm.onElementHeightChangeTimer )
			  clearTimeout(elm.onElementHeightChangeTimer);

			elm.onElementHeightChangeTimer = setTimeout(run, 100);
		})();
	}

	function init(settings) {
		SETTINGS = settings;
		if(SETTINGS.mode == 'sso_login' || SETTINGS.mode == 'oauth_login'){
			if(SETTINGS.platform == PLATFORM_LINE && checkLoginLine()){
				return;
			}
			if(checkLoginVk()){
				return;
			}
			if(SETTINGS.platform == PLATFORM_GOOGLE && checkLoginGoogle()){
				return;
			}
			if(SETTINGS.platform == PLATFORM_HUAWEI && checkLoginHuawei()){
				return;
			}
			if(checkLoginFacebook()){
				return;
			}
			if (SETTINGS.platform == PLATFORM_VK) {
				loginVk();
			} else if(SETTINGS.platform == PLATFORM_LINE) {
				loginLine();
			} else if  (SETTINGS.platform == PLATFORM_GOOGLE) {
				loginGoogle();
			} else if (SETTINGS.platform == PLATFORM_HUAWEI) {
				loginHuawei();
			} else if (SETTINGS.platform == PLATFORM_FACEBOOK){
				loginFacebook();
			} else if (SETTINGS.platform == PLATFORM_BEETALK){
				if (isMobile()) {
					showLoginPageBeetalk('mobile');
				} else {
					showLoginPageBeetalk('pc');
				}
			}else{
				if (isMobile()) {
					showLoginPage('mobile');
				} else {
					showLoginPage('pc');
				}
			}
		}else if(SETTINGS.mode == 'oauth_grant'){
			if (isMobile()){
				showGrantPage('mobile');
			} else {
				showGrantPage('pc');
			}
		}else if(SETTINGS.mode == 'register'){
			if (isMobile()) {
				showRegisterPage('mobile');
			} else {
				showRegisterPage('pc');
			}
		}
		SSO_SERVER.initCallback();
	}

	function setInitCallback(func) {
		SSO_SERVER.initCallback = func;
	}

	window.SSO_SERVER = {};
	var SSO_SERVER = window.SSO_SERVER;
	SSO_SERVER.init = init;
	SSO_SERVER.preloginCallback = preloginCallback;
	SSO_SERVER.loginCallback = loginCallback;
	SSO_SERVER.grantTokenCallback = grantTokenCallback;
	SSO_SERVER.beetalkPreloginCallback = beetalkPreloginCallback;
	SSO_SERVER.beetalkLoginCallback = beetalkLoginCallback;
	SSO_SERVER.beetalkSendOtpCallback = beetalkSendOtpCallback;
	SSO_SERVER.beetalkOtpLoginCallback = beetalkOtpLoginCallback;
	SSO_SERVER.initCallback = function() { };
})();
