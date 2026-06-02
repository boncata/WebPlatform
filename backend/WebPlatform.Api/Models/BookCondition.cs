/*
Book condition class.
*/
namespace WebPlatform.Api.Models;

public enum BookCondition
{
    // New: Never been read, potentially in original packaging, no damage.  
    New = 10,
    // LikeNew: Read once, very gently. Same condition as new. No markings, no damage.
    LikeNew = 20,
    // Excellent: Still in great shape, but shows that it was read. Minor signs of wear.
    Excellent = 30,
    // Good: May have some flaws or signs of wear, but nothing that affects readability.
    Good = 40, 
    // Fair: Show obvious imperfections or damage but are still readable.
    Fair = 50,
    // Poor: Major conditional problems and damage that affect readability.
    Poor = 60,
}
