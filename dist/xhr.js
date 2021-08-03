const reqUrlChapters = 'http://81.177.135.200:322/api/chapters'
const xhr = new XMLHttpRequest()

function sendReq(method, url, responseType, body = null) {
  return new Promise((resolve, reject) => {
    xhr.open(method, url);
    xhr.responseType = responseType;
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.onload = () => {
      xhr.status >= 400 ? reject('invalid request url') : resolve(xhr.response);
    }
    xhr.send(JSON.stringify(body));
  })
  .catch(err => {
    UIChecker(loader, false)
    mainInput.value = '';
    mainInput.setAttribute('placeholder', err);
    webPlayer.style.transition = '1s ease';
    webPlayer.style.display = 'none';
  })
}

function getChapters(url) {
    UIChecker(loader, true)
    sendReq("POST", reqUrlChapters, 'json', {
    url: url,
    includeThumbnails: true
  })
  .then(data => {
    const chapters = data;
    if (data) {
      UIChecker(loader, false);
      UIChecker(webPlayer, true);
      const playerBg = document.querySelector('.player__bg');
      playerSubTitle.innerHTML = chapters[0].name;
      playerBg.setAttribute('src', `data:image/png;base64, ${chapters[0].thumbnailBase64}`);
      let itemsList = '';
      chapters.forEach((item, idx) => {
        itemsList += `
        <li class="nav-list-i">
          <a class="nav__link" id="${idx}" href="#">${chapters[idx].original}</a>
        </li>
        `;
      });
      navList.innerHTML = itemsList;
      const navLinks = document.querySelectorAll('.nav__link');
      console.log(navLinks);
      navLinks.forEach((item, idx) => {
        item.addEventListener('click', () => {
          playerSubTitle.innerHTML = chapters[idx].name;
          playerBg.setAttribute('src', `data:image/png;base64, ${chapters[idx].thumbnailBase64}`)
        })
      })
    }
  })
}
function downloadZip(url, description = null) {
  UIChecker(loader,true);
  sendReq("POST", "http://81.177.135.200:322/api/download/zip", "blob", {
    url: url,
    description: description
  })
  .then(data =>{
    UIChecker(loader,false);
    console.log(data);
      let tempEl = document.createElement("a");
      	document.body.appendChild(tempEl);
    	tempEl.style = "display: none";
        url = window.URL.createObjectURL(data);
        tempEl.href = url;
        tempEl.download = 'file.zip';
        tempEl.click();
		window.URL.revokeObjectURL(url);
  })
}
