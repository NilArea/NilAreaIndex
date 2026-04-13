function isAuthenticated(): boolean {
  return false
}
// ---cut---
export default defineNuxtRouteMiddleware((to, from) => {
  // isAuthenticated() is an example method verifying if a user is authenticated
  if (!isAuthenticated()) {
    return navigateTo('/sign')
  }
})
