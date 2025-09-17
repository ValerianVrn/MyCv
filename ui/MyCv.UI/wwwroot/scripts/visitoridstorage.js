window.getOrCreateVisitorId = function () {
  let id = localStorage.getItem("visitor-id");
  if (!id) {
    id = crypto.randomUUID();
    localStorage.setItem("visitor-id", id);
  }
  return id;
};
