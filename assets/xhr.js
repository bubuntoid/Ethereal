const reqURL = 'http://81.177.135.200:322/api/chapters'
const xhr = new XMLHttpRequest()

function sendReq(method, url, body = null) {
  return new Promise((resolve, reject) => {
    xhr.open(method, url);
    xhr.responseType = 'json';
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.onload = () => {
      xhr.status >= 400 ?  reject('invalid request url') : resolve(xhr.response);
    }
    xhr.send(JSON.stringify(body));
  })
  .catch(err => {
    loaderChecker(false)
    mainInput.value = '';
    mainInput.setAttribute('placeholder', err);
    webPlayer.style.transition = '1s ease';
    webPlayer.style.transform = 'translate(100vw, 0)';
  })
  .then(data => {
    loaderChecker(false)
    chapters = data;
    if (data) {
      webPlayer.style.transition = '1s ease';
      webPlayer.style.transform = 'translate(0, 0)';

      playerSubTitle.innerHTML = chapters[0].name;

      let itemsList = '';
      chapters.forEach((item, idx) => {
        itemsList += `
        <li class="nav-list-i">
          <a class="nav__link" id="${idx}" href="#">${chapters[idx].original}</a>
        </li>
        `;

      });
      navList.innerHTML = itemsList;

      navLinks = document.querySelectorAll('.nav__link');
      navLinks.forEach((item, idx) => {
        item.addEventListener('click', () => {
          playerSubTitle.innerHTML = chapters[idx].name;
        })
      })
    }
  })
}