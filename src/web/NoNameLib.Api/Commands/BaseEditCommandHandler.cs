﻿using AutoMapper;
using NoNameLib.Domain.Interfaces;
using NoNameLib.Domain.Utils.Extensions;
using static NoNameLib.Domain.Utils.TransactionType;

namespace NoNameLib.Api.Commands;

public abstract class BaseAsyncEditCommandHandler<TEditModel, TDomain, TKey> :
    Command,
    IAsyncCommand<TEditModel>
    where TEditModel : class, IDomain<TKey>
    where TDomain : class, IDomain<TKey>
{
    protected readonly IMapper _mapper;
    protected readonly IAsyncRepository<TDomain> _repository;
    protected readonly IAsyncUnitOfWork _unitOfWork;
    protected readonly IAsyncQuery<TDomain> _query;

    protected BaseAsyncEditCommandHandler(
        IMapper mapper,
        IAsyncRepository<TDomain> repository,
        IAsyncUnitOfWork unitOfWork,
        IAsyncQuery<TDomain> query)
    {
        _mapper = mapper;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _query = query;
    }

    public virtual async Task<TEditModel> HandleAsync(
        TEditModel model,
        CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var existingQuery =
                from existing in await _query.GetAsync(cancellationToken)
                where existing.Id.Equals(model.Id)
                select existing;

            var existingDomain = existingQuery.FirstOrDefault();

            if (existingDomain is null)
                throw new NullReferenceException(nameof(existingDomain));

            existingDomain.Update(model);

            var savedRows = await _repository.SaveChangesAsync(existingDomain, Update, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return _mapper.Map<TEditModel>(existingDomain);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}

public abstract class BaseEditCommandHandler<TModel, TDomain, TKey> :
    Command,
    ICommand<TModel>
    where TModel : class, IDomain<TKey>
    where TDomain : class, IDomain<TKey>
{
    protected readonly IRepository<TDomain> _repository;
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IMapper _mapper;
    protected readonly IQuery<TDomain> _query;

    protected BaseEditCommandHandler(
        IRepository<TDomain> repository,
        IUnitOfWork unitOfWork,
        IMapper _mapper,
        IQuery<TDomain> query)
    {
        this._repository = repository;
        this._unitOfWork = unitOfWork;
        this._mapper = _mapper;
        _query = query;
    }

    public virtual TModel Handle(TModel model)
    {
        _unitOfWork.BeginTransaction();

        try
        {
            var existingQuery =
                from existing in _query.Get()
                where existing.Id.Equals(model.Id)
                select existing;

            var existingDomain = existingQuery.FirstOrDefault();

            if (existingDomain is null)
                throw new NullReferenceException(nameof(existingDomain));

            existingDomain.Update(model);

            var savedRows = _repository.SaveChanges(existingDomain, Update);
            _unitOfWork.Commit();
            return _mapper.Map<TModel>(existingDomain);
        }
        catch
        {
            _unitOfWork.RollbackTransaction();
            throw;
        }
    }
}
