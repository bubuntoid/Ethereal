const footer = document.querySelector('.footer');
const mainInput = document.querySelector('.main__input-text');

let footerChecker = (isOpen = false) => {
  footer.style.transition = '1s ease';
  isOpen ? footer.style.transform = 'translate(0, -30vh)' : footer.style.transform = 'translate(0, 0vh)';
}

function input2req(url) {
  url = mainInput.value
  console.log(url)
  sendReq('POST', reqURL, {
    url: url
  })
}
