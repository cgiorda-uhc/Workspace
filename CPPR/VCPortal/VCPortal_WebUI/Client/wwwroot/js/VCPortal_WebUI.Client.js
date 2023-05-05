window.getTitle = () => {
    return document.title;
};



window.setTitle = (title) => {
    document.title = title;
};
 

window.setTagValue = (tagid, value) => {
    document.getElementById(tagid).textContent = value;
};


//https://stackoverflow.com/questions/52683706/how-can-one-generate-and-save-a-file-client-side-using-blazor-->
function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}