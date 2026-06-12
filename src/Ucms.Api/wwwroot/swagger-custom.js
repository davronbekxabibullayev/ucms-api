/**
 * UCMS Swagger UI — Custom Auth Integration
 * Authorize tugmasi bosilganda login sahifasini ochadi
 * va token avtomatik Swagger'ga qo'llanadi.
 */
(function () {
  'use strict';

  // Sahifa yuklanganda localStorage'dan tokenni oladi va Swagger'ga qo'llaydi
  function applyStoredToken() {
    const token = localStorage.getItem('ucms_access_token');
    if (token && window.ui) {
      window.ui.preauthorizeApiKey('Bearer', token);
    }
    updateLogoutButton();
  }

  // Logout tugmasini ko'rsatadi/yashiradi
  function updateLogoutButton() {
    const btn = document.getElementById('ucms-logout-btn');
    if (!btn) return;
    btn.style.display = localStorage.getItem('ucms_access_token') ? 'block' : 'none';
  }

  // Logout
  function logout() {
    localStorage.removeItem('ucms_access_token');
    if (window.ui) { window.ui.preauthorizeApiKey('Bearer', ''); }
    updateLogoutButton();
    showToast('🚪 Token o\'chirildi. Tizimdan chiqdingiz.');
  }

  // Logout tugmasini Authorize tugmasi yoniga (auth-wrapper ichiga) qo'shadi
  function injectLogoutButton() {
    if (document.getElementById('ucms-logout-btn')) { updateLogoutButton(); return; }
    const wrapper = document.querySelector('.auth-wrapper');
    if (!wrapper) return; // hali render bo'lmagan

    const btn = document.createElement('button');
    btn.id = 'ucms-logout-btn';
    btn.className = 'btn authorize';
    btn.textContent = '🚪 Logout';
    btn.style.cssText = [
      'display:none', 'margin-left:10px',
      'background:#e53e3e', 'color:#fff',
      'border:2px solid #e53e3e', 'border-radius:4px',
      'padding:6px 16px', 'font-size:14px',
      'font-weight:700', 'cursor:pointer',
    ].join(';');
    btn.addEventListener('mouseenter', function () { btn.style.background = '#c53030'; btn.style.borderColor = '#c53030'; });
    btn.addEventListener('mouseleave', function () { btn.style.background = '#e53e3e'; btn.style.borderColor = '#e53e3e'; });
    btn.addEventListener('click', function (e) { e.stopPropagation(); logout(); });
    wrapper.appendChild(btn);
    updateLogoutButton();
  }

  // Swagger UI render bo'lishini kuzatadi va tugmani qo'shadi
  function watchAndInject() {
    if (document.querySelector('.auth-wrapper')) {
      injectLogoutButton();
      return;
    }
    const observer = new MutationObserver(function () {
      if (document.querySelector('.auth-wrapper')) {
        observer.disconnect();
        injectLogoutButton();
      }
    });
    observer.observe(document.body, { childList: true, subtree: true });
  }

  // Login sahifasidan postMessage orqali token qabul qiladi
  window.addEventListener('message', function (e) {
    if (e.data && e.data.ucmsToken) {
      const token = e.data.ucmsToken;
      localStorage.setItem('ucms_access_token', token);
      applyStoredToken();
      injectLogoutButton();
      showToast('✅ Token muvaffaqiyatli qo\'yildi!');
    }
  });

  // Authorize tugmasini ushlab qoladi (capture phase)
  document.addEventListener('click', function (e) {
    const authorizeBtn = e.target.closest('.btn.authorize');
    if (!authorizeBtn) return;

    e.stopImmediatePropagation();

    const w = 500, h = 640;
    const left = Math.round((screen.width  - w) / 2);
    const top  = Math.round((screen.height - h) / 2);
    const popup = window.open(
      '/auth/login',
      'ucms-login',
      'width=' + w + ',height=' + h + ',left=' + left + ',top=' + top + ',resizable=yes'
    );

    if (!popup) {
      // Popup bloklangan holda yangi tab ochadi
      window.open('/auth/login', '_blank');
      showToast('ℹ️ Login sahifasi yangi tabda ochildi. Token olingach Swagger\'ga qo\'llang.');
      return;
    }

    // Token kelishini kutish (popup yopilgunga qadar)
    const poll = setInterval(function () {
      const token = localStorage.getItem('ucms_access_token');
      if (token && window.ui) {
        clearInterval(poll);
        window.ui.preauthorizeApiKey('Bearer', token);
        injectLogoutButton();
        showToast('✅ Token qo\'yildi! Endi API\'larni test qilishingiz mumkin.');
        if (popup && !popup.closed) popup.close();
      }
      if (popup.closed) clearInterval(poll);
    }, 600);
  }, true /* capture phase */);

  // Toast xabari ko'rsatadi
  function showToast(msg) {
    let toast = document.getElementById('ucms-toast');
    if (!toast) {
      toast = document.createElement('div');
      toast.id = 'ucms-toast';
      toast.style.cssText = [
        'position:fixed', 'bottom:24px', 'right:24px', 'z-index:99999',
        'background:#1a1a2e', 'color:#fff', 'padding:12px 20px',
        'border-radius:8px', 'font-size:14px', 'font-weight:600',
        'box-shadow:0 4px 20px rgba(0,0,0,.4)', 'transition:opacity .3s',
      ].join(';');
      document.body.appendChild(toast);
    }
    toast.textContent = msg;
    toast.style.opacity = '1';
    clearTimeout(toast._hide);
    toast._hide = setTimeout(function () { toast.style.opacity = '0'; }, 3500);
  }

  // Sahifa yüklenganda mavjud tokenni qo'llaydi
  window.addEventListener('load', function () {
    watchAndInject();
    setTimeout(applyStoredToken, 1200);
  });
})();
