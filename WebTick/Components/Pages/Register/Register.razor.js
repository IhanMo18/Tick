 // ===== Tema =====
    const themeToggle = document.getElementById('themeToggle');
    const themeIcon = document.getElementById('themeIcon');
    const body = document.body;
    const savedTheme = localStorage.getItem('theme');
    if(savedTheme){
    body.classList.toggle('dark', savedTheme==='dark');
    body.classList.toggle('light', savedTheme!=='dark');
    themeIcon.classList.toggle('fa-sun', savedTheme==='dark');
    themeIcon.classList.toggle('fa-moon', savedTheme!=='dark');
} else {
    body.classList.add('light');
}
    themeToggle?.addEventListener('click',()=>{
    const toDark = !body.classList.contains('dark');
    body.classList.toggle('dark', toDark);
    body.classList.toggle('light', !toDark);
    localStorage.setItem('theme', toDark ? 'dark' : 'light');
    themeIcon.classList.toggle('fa-sun', toDark);
    themeIcon.classList.toggle('fa-moon', !toDark);
});

    // ===== Utilidades
    const $ = s => document.querySelector(s);
    const $$ = s => document.querySelectorAll(s);

    const forms = [$('#step0'), $('#step1'), $('#step2'), $('#step3')];
    const tabs  = [$('#tab0'), $('#tab1'), $('#tab2'), $('#tab3')];
    const slider = $('#slider');
    let current = 0;

    function moveSliderTo(idx){
    const wrap = $('#stepper').querySelector('.tabs');
    const target = tabs[idx];
    const wrapRect = wrap.getBoundingClientRect();
    const rect = target.getBoundingClientRect();
    slider.style.left  = (rect.left - wrapRect.left + 8) + 'px'; // +gap
    slider.style.width = (rect.width - 16) + 'px';
    tabs.forEach((t,i)=> t.setAttribute('data-active', i===idx ? 'true':'false'));
}
    function showStep(n, dir='right'){
    forms[current].classList.add('hidden');
    current = n;
    forms[current].classList.remove('hidden');
    forms[current].classList.add(dir==='right' ? 'in-right' : 'in-left');
    moveSliderTo(current);
    window.scrollTo({top:0, behavior:'smooth'});
}
    window.addEventListener('resize', ()=> moveSliderTo(current));
    moveSliderTo(0);
    setTimeout(()=> moveSliderTo(0), 50);

    // ===== Paso 0: validación edad 13–17
    const fechaNac = $('#fechaNac'), edadMsg = $('#edadMsg'), to1 = $('#to1');
    const calcEdad = d => {
    if(!d) return null;
    const t = new Date(), x = new Date(d);
    let a = t.getFullYear()-x.getFullYear();
    const m=t.getMonth()-x.getMonth();
    if(m<0||(m===0 && t.getDate()<x.getDate())) a--;
    return a;
};
    function val0(){
    const alias = $('#alias').value.trim();
    const ciudad = $('#ciudad').value.trim();
    const e = calcEdad(fechaNac.value);
    let ok = !!alias && !!ciudad && e!==null;
    if(e===null){ edadMsg.textContent=''; to1.disabled=true; return false; }
    if(e<13 || e>17){
    edadMsg.textContent='Debes tener entre 13 y 17 años.';
    edadMsg.className='mt-2 text-sm text-red';
    ok=false;
} else {
    edadMsg.textContent=`Edad verificada: ${e} años.`;
    edadMsg.className='mt-2 text-sm text-green';
}
    to1.disabled=!ok; return ok;
}
    ['input','change'].forEach(ev=>{
    $('#alias').addEventListener(ev,val0);
    $('#ciudad').addEventListener(ev,val0);
    fechaNac.addEventListener(ev,val0);
});
    to1.addEventListener('click', e=>{
    e.preventDefault(); if(val0()) showStep(1,'right');
});

    // ===== Paso 1: info
    const bio = $('#bio'), bioCount = $('#bioCount');
    bio.addEventListener('input',()=> bioCount.textContent = bio.value.length);

    const avatarInput = $('#avatar'), avatarPrev = $('#avatarPreview'), avatarIcon = $('#avatarIcon');
    avatarInput.addEventListener('change',e=>{
    const f = e.target.files?.[0]; if(!f) return;
    const url = URL.createObjectURL(f);
    avatarPrev.src = url; avatarPrev.classList.remove('hidden'); avatarIcon.classList.add('hidden');
});

    const intereses = [];
    const interesInput = $('#interesInput'), interesesBox = $('#interesesBox'), addInteresBtn = $('#addInteres');
    function renderIntereses(){
    interesesBox.innerHTML='';
    intereses.forEach((t,i)=>{
    const s = document.createElement('span');
    s.className='chip';
    s.innerHTML = `${t} <i class="fa fa-times" style="margin-left:6px"></i>`;
    s.title='Quitar';
    s.style.cursor='pointer';
    s.addEventListener('click',()=>{intereses.splice(i,1); renderIntereses();});
    interesesBox.appendChild(s);
});
}
    function addInteres(){
    const v = interesInput.value.trim(); if(!v) return;
    if(intereses.length>=8) return alert('Máximo 8 intereses.');
    if(intereses.includes(v)) return;
    intereses.push(v); interesInput.value=''; renderIntereses();
}
    interesInput.addEventListener('keydown', e=>{ if(e.key==='Enter'){ e.preventDefault(); addInteres(); }});
    addInteresBtn.addEventListener('click', addInteres);

    $('#back0').addEventListener('click', ()=> showStep(0,'left'));
    $('#to2').addEventListener('click', e=>{ e.preventDefault(); showStep(2,'right'); });

    // ===== Paso 2: preferencias
    const prefBtns = Array.from($('#step2').querySelectorAll('button[data-opt]'));
    const prefs = new Set();
    prefBtns.forEach(btn=>{
    btn.addEventListener('click', ()=>{
        const k = btn.dataset.opt;
        const pressed = btn.getAttribute('aria-pressed') === 'true';
        if(pressed){
            prefs.delete(k);
            btn.setAttribute('aria-pressed','false');
        } else {
            prefs.add(k);
            btn.setAttribute('aria-pressed','true');
        }
    });
});

    const ageMin = $('#ageMin'), ageMax = $('#ageMax'), agePrefMsg = $('#agePrefMsg');
    function valAges(){
    let min = parseInt(ageMin.value||'13',10), max = parseInt(ageMax.value||'17',10);
    if(min<13) min=13; if(max>17) max=17; if(min>max){ [min,max]=[max,min]; }
    ageMin.value=min; ageMax.value=max;
    agePrefMsg.textContent = `Mostraremos perfiles de ${min} a ${max} años.`;
    agePrefMsg.className='text-sm text-green';
}
    ageMin.addEventListener('input',valAges); ageMax.addEventListener('input',valAges); valAges();

    $('#back1').addEventListener('click', ()=> showStep(1,'left'));
    $('#to3').addEventListener('click', e=>{ e.preventDefault(); showStep(3,'right'); });

    // ===== Paso 3: cuenta
    const email = $('#email'), emailMsg = $('#emailMsg');
    const pwd = $('#pwd'), pwd2 = $('#pwd2'), pwdMsg = $('#pwdMsg'), pwd2Msg = $('#pwd2Msg'), meter = $('#meter');
    const consent = $('#consent'), save = $('#save');

    function emailOk(){
    const ok = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value);
    emailMsg.textContent = ok ? 'Correo válido.' : 'Correo inválido.';
    emailMsg.className = 'text-sm ' + (ok ? 'text-green':'text-red');
    return ok;
}
    email.addEventListener('input',()=> { emailOk(); enableSave(); });

    function scorePwd(s){
    let n=0; if(s.length>=8) n++; if(/[A-Z]/.test(s)) n++; if(/[a-z]/.test(s)) n++; if(/[0-9]/.test(s)) n++; if(/[^A-Za-z0-9]/.test(s)) n++;
    return Math.min(n,5);
}
    function updatePwd(){
    const s = pwd.value;
    const k = scorePwd(s);
    meter.style.width = (k*20)+'%';
    meter.style.background = (k<=2)?'#ef4444':(k===3)?'#eab308':(k===4)?'#84cc16':'#22c55e';
    pwdMsg.textContent = s.length<8 ? 'Mínimo 8 caracteres.' : 'Fuerza contraseña OK.';
    pwdMsg.className = 'text-sm ' + (s.length<8?'text-red':'text-green');
    matchPwd();
    enableSave();
}
    function matchPwd(){
    const ok = pwd.value && pwd.value===pwd2.value;
    pwd2Msg.textContent = ok?'Coincide.':'No coincide.';
    pwd2Msg.className = 'text-sm ' + (ok?'text-green':'text-red');
    return ok;
}
    pwd.addEventListener('input',updatePwd);
    pwd2.addEventListener('input',()=>{ matchPwd(); enableSave(); });

    $('#togglePwd').addEventListener('click',()=>{
    pwd.type = pwd.type==='password' ? 'text' : 'password';
    $('#togglePwd').textContent = pwd.type==='password' ? 'Ver':'Ocultar';
});
    consent.addEventListener('change',()=> enableSave());

    function enableSave(){
    const ok = emailOk() && pwd.value.length>=8 && matchPwd() && consent.checked;
    save.disabled = !ok;
}

    $('#back2').addEventListener('click', ()=> showStep(2,'left'));

    $('#save').addEventListener('click', e=>{
    e.preventDefault(); if(save.disabled) return;
    const data = {
    alias: $('#alias').value.trim(),
    edad: (function(){
    const d = $('#fechaNac').value; if(!d) return null;
    const t=new Date(), x=new Date(d);
    let a=t.getFullYear()-x.getFullYear();
    const m=t.getMonth()-x.getMonth();
    if(m<0||(m===0&&t.getDate()<x.getDate())) a--;
    return a;
})(),
    ciudad: $('#ciudad').value.trim(),
    bio: $('#bio').value.trim(),
    intereses,
    prefs: Array.from(prefs),
    agePref: {min: parseInt(ageMin.value,10), max: parseInt(ageMax.value,10)},
    onlyCommon: $('#onlyCommon').checked,
    suggestions: $('#suggestions').checked,
    email: email.value
};
    localStorage.setItem('perfilSafeTeen', JSON.stringify(data));
    const toast = $('#toast'); toast.classList.remove('hidden');
    setTimeout(()=> window.location.href='index.html', 1400);
});

    // Tabs clicables (solo hacia atrás)
    tabs.forEach((t,idx)=>{
    t.addEventListener('click', ()=>{
        if(idx < current) showStep(idx,'left'); // evita saltar hacia adelante sin datos
    });
});