using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository addressRepository, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AddressDto>> GetAllByUserIdAsync(int userId)
        {
            var addresses = await _addressRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<AddressDto>>(addresses);
        }

        public async Task<AddressDto?> GetByIdAsync(int id)
        {
            var address = await _addressRepository.GetByAsync(id);
            return _mapper.Map<AddressDto?>(address);
        }

        public async Task<AddressDto> AddAsync(AddressDto addressDto)
        {
            var entity = _mapper.Map<Address>(addressDto);
            var created = await _addressRepository.AddAsync(entity);
            return _mapper.Map<AddressDto>(created);
        }

        public async Task<AddressDto> UpdateAsync(AddressDto addressDto)
        {
            var entity = _mapper.Map<Address>(addressDto);
            var updated = await _addressRepository.UpdateAsync(entity);
            return _mapper.Map<AddressDto>(updated);
        }

        public async Task DeleteAsync(int id)
        {
            await _addressRepository.DeleteAsync(id);
        }
    }
}
